using HarmonyLib;

namespace Essentials.UI
{
    [HarmonyPatch]
    public partial class CooldownButton
    {
        [HarmonyPatch(typeof(HudManager._CoShowIntro_d__56), nameof(HudManager._CoShowIntro_d__56.MoveNext))]
        [HarmonyPostfix]
        private static void HudManagerCoShowIntro()
        {
            const float fadeTime = 0.2F;
            foreach (CooldownButton button in CooldownButtons) button.ApplyCooldown(button.InitialCooldownDuration - fadeTime); // Match start cooldown.
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        [HarmonyPostfix]
        private static void MeetingHudStart()
        {
            foreach (CooldownButton button in CooldownButtons) if (button.MeetingsEndEffect) button.EndEffect(false); // End button effect early.
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
        [HarmonyPostfix]
        private static void ExileControllerWrapUp()
        {
            if (!DestroyableSingleton<TutorialManager>.InstanceExists && ShipStatus.Instance.IsGameOverDueToDeath()) return;

            foreach (CooldownButton button in CooldownButtons) if (button.CooldownAfterMeetings && !button.IsEffectActive) button.ApplyCooldown(); // Set button on cooldown after exile screen.
        }

        //game start/end to reset effects/cooldowns
    }
}