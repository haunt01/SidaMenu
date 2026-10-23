using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SidaMenu.Patches
{
    public class UnlockSkinsPatch // everything is from https://github.com/AstralKitsune/MalumMenu/blob/reloaded/src/Cheats/CosmeticsUnlocker.cs
    {
        [HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
        public static class HatManager_Initialize
        {
            public static void Postfix(HatManager __instance)
            {
                UnlockSkins(__instance);
            }
        }
        public static void UnlockSkins(HatManager hatManager)
        {
            if (!States.unlockSkins) return;

            foreach (var bundle in hatManager.allBundles)
            {
                bundle.Free = true;
            }

            foreach (var featuredBundle in hatManager.allFeaturedBundles)
            {
                featuredBundle.Free = true;
            }

            foreach (var featuredCube in hatManager.allFeaturedCubes)
            {
                featuredCube.Free = true;
            }

            foreach (var featuredItem in hatManager.allFeaturedItems)
            {
                featuredItem.Free = true;
            }

            foreach (var hat in hatManager.allHats)
            {
                hat.Free = true;
            }

            foreach (var nameplate in hatManager.allNamePlates)
            {
                nameplate.Free = true;
            }

            foreach (var pet in hatManager.allPets)
            {
                pet.Free = true;
            }

            foreach (var skin in hatManager.allSkins)
            {
                skin.Free = true;
            }

            foreach (var visor in hatManager.allVisors)
            {
                visor.Free = true;
            }
        }
    }
}
