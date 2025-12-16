using Application.Abstractions;
using Contract.Requests;
using Contract.Responses;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services
{
    public class MembresiaService : IMembresiaService
    {
        private readonly IMembresiaRepository _membresiaRepository;
        private readonly IAlumnoRepository _alumnoRepository; // Nota: Esta dependencia puede no usarse si solo usas IUsuarioRepository
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IPagoService _pagoService;

        public MembresiaService(
            IMembresiaRepository membresiaRepository,
            IAlumnoRepository alumnoRepository,
            IUsuarioRepository usuarioRepository,
            IPlanRepository planRepository,
            IPagoService pagoService
        )
        {
            _membresiaRepository = membresiaRepository;
            _alumnoRepository = alumnoRepository;
            _usuarioRepository = usuarioRepository;
            _planRepository = planRepository;
            _pagoService = pagoService;
        }
        
        // ***** MÉTODO CLAVE MODIFICADO: Devuelve string (error) o null (éxito) *****
        // El Controller lo llama para obtener el mensaje de error detallado.
        public string? AsociarMembresia(CreateMembresiaRequest request)
        {
            // 1. Validar que el usuario exista y esté activo
            if (!_usuarioRepository.IsActivo(request.AlumnoId))
                return "El alumno no existe o no está activo.";

            // 2. Validar el plan
            var plan = _planRepository.GetById(request.PlanId);
            if (plan == null || !plan.Activo)
                return "El plan seleccionado no existe o no está disponible.";

            // 3. Validar si ya tiene membresía activa
            // CORRECCIÓN: Esto debe devolver un string de error, no un bool en esta función.
            if (_membresiaRepository.TieneMembresiaActiva(request.AlumnoId))
                return "El alumno ya tiene una membresía activa y vigente. Debe esperar a que expire o gestionarla manualmente.";

            var membresia = new Membresia
            {
                AlumnoId = request.AlumnoId,
                PlanId = request.PlanId,
                FechaInicio = DateOnly.FromDateTime(DateTime.Today),
                // Asume que Plan tiene una propiedad DuracionDias
                FechaFin = DateOnly.FromDateTime(DateTime.Today).AddDays(plan.DuracionDias), 
                Activa = true,
            };

            // 4. Intento de creación en BD
            if (!_membresiaRepository.Create(membresia))
                return "Error interno al guardar la membresía en la base de datos (posiblemente un dato faltante o inválido).";

            _pagoService.RegistrarPagoInicial(membresia.Id, plan.Precio);

            return null; // Éxito
        }
        
        // ***** MÉTODO REQUERIDO POR LA INTERFACE IMembresiaService (Retorna bool) *****
        public bool Create(CreateMembresiaRequest request)
        {
            // Llama a la nueva lógica y retorna true si no hubo errores (error == null)
            return AsociarMembresia(request) == null;
        }
        // ********************************************************************************

        public bool Update(int id, UpdateMembresiaRequest request)
        {
            var membresia = _membresiaRepository.GetById(id);
            if (membresia == null)
                return false;

            if (request.PlanId.HasValue)
                membresia.PlanId = request.PlanId.Value;

            if (request.FechaInicio.HasValue)
                membresia.FechaInicio = request.FechaInicio.Value;

            if (request.FechaFin.HasValue)
                membresia.FechaFin = request.FechaFin.Value;

            if (request.Activa.HasValue)
                membresia.Activa = request.Activa.Value;

            return _membresiaRepository.Update(membresia);
        }

        public List<MembresiaResponse> GetByAlumnoId(int alumnoId)
        {
            var membresias = _membresiaRepository.GetByCriterial(m => m.AlumnoId == alumnoId);

            return membresias
                .Select(m => new MembresiaResponse
                {
                    Id = m.Id,
                    PlanId = m.PlanId,
                    AlumnoId = m.AlumnoId,
                    // Asegura formato ISO 8601 con UTC para compatibilidad con JS
                    FechaInicio = m
                        .FechaInicio.ToDateTime(TimeOnly.MinValue)
                        .ToUniversalTime()
                        .ToString("o"),
                    FechaFin = m
                        .FechaFin.ToDateTime(TimeOnly.MaxValue)
                        .ToUniversalTime()
                        .ToString("o"),
                    Estado =
                        m.Activa && m.FechaFin >= DateOnly.FromDateTime(DateTime.Today)
                            ? "activa"
                            : "expirada",
                    Activa = m.Activa,
                })
                .ToList();
        }
    }
}