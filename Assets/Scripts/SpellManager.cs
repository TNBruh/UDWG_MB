using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellManager : MonoBehaviour
{
    internal abstract IEnumerator Init();

    internal abstract bool Finished();

    internal abstract IEnumerator CleanUp();
}
