using System;

namespace DDDCqrsEs.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MapServiceDependency : Attribute
    {
        protected string _name;
        public string Name => _name;
        public MapServiceDependency(string Name)
        {
            this._name = Name;
        }
    }
}
