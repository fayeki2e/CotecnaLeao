using System;
using System.Collections.Generic;
using System.Text;

namespace Techparva.GenericRepository
{
    public interface IModifiableEntity
    {
    }

    public interface IEntity : IModifiableEntity
    {
        object Id { get; set; }
    }

    public interface IEntity<T> : IEntity
    {
        new T Id { get; set; }
    }
}
