using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;
using System;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios
{
    public class RepositorioByPass : Repositorio<BY_PASS>, IRepositorioByPass
    {
        public void InsertarRegistro(BY_PASS byPass)
        {
            var preGate = Contexto.PRE_GATES.FirstOrDefault(pg => pg.PRE_GATE_ID == byPass.PRE_GATE.PRE_GATE_ID);
            byPass.PRE_GATE = preGate ?? throw new ApplicationException($"No existe Id {byPass.PRE_GATE.PRE_GATE_ID}");
            Contexto.BY_PASSES.Add(byPass);
            Contexto.SaveChanges();
        }
    }
}
