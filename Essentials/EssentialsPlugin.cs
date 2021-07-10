using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using Essentials.UI;
using HarmonyLib;
using Reactor;
using Reactor.Patches;
using System.Reflection;

namespace Essentials
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id, BepInDependency.DependencyFlags.HardDependency)]
    [ReactorPluginSide(PluginSide.Both)]
    public partial class EssentialsPlugin : BasePlugin
    {
        public const string Id = "com.comando.essentials";

        public static EssentialsPlugin Instance { get { return PluginSingleton<EssentialsPlugin>.Instance; } }

        internal static ManualLogSource Logger { get { return Instance.Log; } }

        internal Harmony Harmony { get; } = new Harmony(Id);

        public override void Load()
        {
            Harmony.PatchAll();
            ReactorVersionShower.TextUpdated += (text) =>
            {
                string txt = text.text;

                int index = txt.IndexOf('\n');
                txt = txt.Insert(index == -1 ? txt.Length - 1 : index, "\nEssentials " + typeof(EssentialsPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion); text.text = txt;
            };

            HudPosition.Load();
        }

#if S202103313
        /// <summary>
        /// Corrects the issue where players are unable to move after meetings in 2021.3.31.3s, may interrupt controller support.
        /// </summary>
        public static bool PatchCanMove { get; set; } = true;

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
        private static class PlayerControlCanMovePatch
        {
            public static bool Prefix(PlayerControl __instance, ref bool __result)
            {
                if (!PatchCanMove) return true;

                __result = __instance.moveable && !Minigame.Instance && (!DestroyableSingleton<HudManager>.InstanceExists || !DestroyableSingleton<HudManager>.Instance.Chat.IsOpen && !DestroyableSingleton<HudManager>.Instance.KillOverlay.IsOpen && !DestroyableSingleton<HudManager>.Instance.GameMenu.IsOpen) /*&& (!ControllerManager.Instance || !ControllerManager.Instance.IsUiControllerActive)*/ && (!MapBehaviour.Instance || !MapBehaviour.Instance.IsOpenStopped) && !MeetingHud.Instance && !CustomPlayerMenu.Instance && !ExileController.Instance && !IntroCutscene.Instance;

                return false;
            }
        }
#endif
    }
}