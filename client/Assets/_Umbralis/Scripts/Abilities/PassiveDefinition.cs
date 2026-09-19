using UnityEngine;

namespace Umbralis.Abilities
{
    /// <summary>
    /// Pasiva de una especialización (Muralla, Sed de batalla...). Al activar la
    /// especialización se le pide que añada su componente al personaje; al
    /// cambiar, que lo quite. Cada pasiva concreta hereda y crea el suyo.
    /// </summary>
    public abstract class PassiveDefinition : ScriptableObject
    {
        public string displayName = "Pasiva";
        [TextArea] public string description;

        /// <summary>Añade el comportamiento al personaje y lo devuelve para poder quitarlo después.</summary>
        public abstract Behaviour Attach(GameObject owner, SpecializationDefinition spec);

        public virtual void Detach(Behaviour attached)
        {
            if (attached != null) Object.Destroy(attached);
        }
    }
}
