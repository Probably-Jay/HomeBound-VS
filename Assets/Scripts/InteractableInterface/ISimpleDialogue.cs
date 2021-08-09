using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{

    public interface ISimpleDialogue
    {
        string Text { get; }
        string Title { get; }

    }
}