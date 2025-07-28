using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio
{
    public interface IRepositorio<TEntity> where TEntity : class
    {
        void Agregar(TEntity objeto);
        void Actualizar(TEntity objeto);
        IEnumerable<TEntity> ObtenerObjetos(IFiltros<TEntity> filtro);
        IEnumerable<TEntity> ObtenerObjetosPorPagineo<S>(IFiltros<TEntity> filtro, int paginaIndice, int elementosPorPagina, Expression<Func<TEntity, S>> expresionOrdenamiento, bool ascendente);
        void LiberarRecursos();
    }
}
