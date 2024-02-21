using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropertyAttributes
{
    // https://github.com/lordofduct/spacepuppy-unity-framework

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class GenericMaskAttribute : PropertyAttribute
    {
        private string[] _maskNames;

        public GenericMaskAttribute(params string[] names)
        {
            _maskNames = names;
        }

        public string[] MaskNames { get { return _maskNames; } }

    }
}
