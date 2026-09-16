using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Androids
{
    public static class MatrixHelpers
    {
        public static Vector3 GetTranslation(this Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        public static bool TryGetRotation(this Matrix4x4 matrix, out Quaternion rotation)
        {
            Vector3 forward = matrix.GetColumn(2);
            if (forward.sqrMagnitude == 0f)
            {
                rotation = Quaternion.identity;
                return false;
            }

            Vector3 up = matrix.GetColumn(1);
            if (up.sqrMagnitude == 0f)
            {
                rotation = Quaternion.LookRotation(forward);
                return false;
            }

            rotation = Quaternion.LookRotation(forward, up);
            return true;
        }

        public static bool TryDecomposeTRS(this Matrix4x4 matrix,
               out Vector3 translation, out Quaternion rotation, out Vector3 scale)
        {
            translation = matrix.GetTranslation();
            scale = matrix.lossyScale;

            return (matrix.TryGetRotation(out rotation) && matrix.ValidTRS());
        }
    }
}
