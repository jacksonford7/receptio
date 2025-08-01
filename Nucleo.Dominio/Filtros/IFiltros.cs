﻿using System;
using System.Linq.Expressions;

namespace RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros
{
    public interface IFiltros<TEntity> where TEntity : class
    {
        Expression<Func<TEntity, bool>> SastifechoPor();
    }
}
