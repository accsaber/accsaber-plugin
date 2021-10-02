using System;

namespace AccSaber.HarmonyPatches
{
    class UpdateSecondChildControllerContentPatch
    {
        internal static event Action SecondChildControllerUpdatedEvent;
        internal static void Postfix()
        {
            SecondChildControllerUpdatedEvent?.Invoke();
        }
    }
}
