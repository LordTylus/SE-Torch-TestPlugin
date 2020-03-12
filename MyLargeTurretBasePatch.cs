using NLog;
using Sandbox.Game.Weapons;
using System;
using System.Reflection;
using Torch.Managers.PatchManager;
using Torch.Utils;

namespace TestPlugin {

    class MyLargeTurretBasePatch {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        internal static readonly MethodInfo update =
            typeof(MyLargeTurretBase).GetMethod(nameof(MyLargeTurretBase.UpdateAfterSimulation10), BindingFlags.Instance | BindingFlags.Public) ??
            throw new Exception("Failed to find patch method");

        internal static readonly MethodInfo updatePatch =
            typeof(MyLargeTurretBasePatch).GetMethod(nameof(TestPatchMethod), BindingFlags.Static | BindingFlags.Public) ??
            throw new Exception("Failed to find patch method");

        public static void Patch(PatchContext ctx) {

            ReflectedManager.Process(typeof(MyLargeTurretBasePatch));

            try {

                ctx.GetPattern(update).Prefixes.Add(updatePatch);

                Log.Info("Patching Successful MyLargeTurretBase!");

            } catch (Exception e) {
                Log.Error(e, "Patching failed!");
            }
        }

        public static bool TestPatchMethod(MyLargeTurretBase __instance) {
            __instance.Enabled = !__instance.Enabled;

            return false;
        }
    }
}
