using NLog;
using Sandbox.Game.Weapons;
using Sandbox.Game.World;
using System;
using System.Linq;
using System.Reflection;
using Torch.Managers.PatchManager;
using Torch.Utils;
using VRage.Game;

namespace TestPlugin {

    class MySessionPatch {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        internal static readonly MethodInfo update =
            typeof(MySession).GetMethod("GetWorld", BindingFlags.Instance | BindingFlags.Public) ??
            throw new Exception("Failed to find MySession.GetWorld method to patch");

        internal static readonly MethodInfo updatePatch =
            typeof(MySessionPatch).GetMethod(nameof(SuffixGetWorld), BindingFlags.Static | BindingFlags.Public) ??
            throw new Exception("Failed to find patch method");

        public static void Patch(PatchContext ctx) {

            ReflectedManager.Process(typeof(MySessionPatch));

            try {

                ctx.GetPattern(update).Suffixes.Add(updatePatch);

                Log.Info("Patching Successful MySessionPatch!");

            } catch (Exception e) {
                Log.Error(e, "Patching failed!");
            }
        }

        public static void SuffixGetWorld(ref MyObjectBuilder_World __result) {
            __result.Checkpoint.Mods = __result.Checkpoint.Mods.ToList();
            __result.Checkpoint.Mods.Add(new MyObjectBuilder_Checkpoint.ModItem(1470445959UL));
        }
    }
}
