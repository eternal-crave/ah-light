using UnityEngine;

namespace Runtime.Services
{
    public interface IInputService
    {
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool IsSprintPressed { get; }
        bool IsJumpPressed { get; }
    }
}

