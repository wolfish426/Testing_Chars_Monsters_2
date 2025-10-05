using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;

public class MonstersDemoScript : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    float buttonSize = 160f;
    ExposedList<Skin> skinsList = new ExposedList<Skin>();
    int currentSkin = 1;
    int headSkin = 1;
    int bodySkin = 1;
    int armsSkin = 1;
    int legsSkin = 1;
    int originalSkinsCount = 22;
    Spine.Skin mutantSkin = null;
    Texture2D tex = null;
    Vector2 scrollPosition;

    public DemoAtlasRegionAttacher demoAtlasRegionAttacher;
    public BottleAtlasRegionAttacher bottleAtlasRegionAttacher;

    public static Dictionary<string, List<string>> partsHash = new Dictionary<string, List<string>>()
    {
        {
            "Head", new List<string>
            {
                "Head",
                "Head2",
                "Eye_open",
                "Eye_closed",
                "Helm" // helm need to update too
            }
        },
        {
            "Body", new List<string>
            {
                "Body",
                "Tail"
            }
        },
        {
            "Legs", new List<string>
            {
                "L_leg_1",
                "L_leg_2",
                "L_leg_3",
                "R_leg_1",
                "R_leg_2",
                "R_leg_3",
                "L_leg_3_Walk",
            }
        },
        {
            "Arms", new List<string>
            {
                "L_hand_1",
                "L_hand_2",
                "L_hand_3",
                "R_hand_1",
                "R_hand_2",
                "R_hand_3"
            }
        },
        {
            "Chest", new List<string>
            {
                "Chest"
            }
        },
        {
            "Shoulders", new List<string>
            {
                "shoulder_L",
                "shoulder_R"
            }
        },
        {
            "Helm", new List<string>
            {
                "Helm"
            }
        },
        /*
        { "Pet", new List<string> {
                "Body",
                "Head",
                "Eye",
                "Leg1",
                "Leg2",
                "Leg3",
                "Leg4",
            } },
            //*/
    };

    public static Dictionary<string, List<string>> superHitHashMap = new Dictionary<string, List<string>>()
    {
        {
            "Arm_Super_Claw_1", new List<string>
            {
                "Crab",
                "Scorpion",
                "default",
                "Fake"
            }
        },
        {
            "Arm_Super_Claw_2", new List<string>
            {
                "Scarecrow",
                "Tree",
            }
        },
        {
            "Arm_Super_Cut_1", new List<string>
            {
                "Crocodile",
                "Ghost",
                "Raven",
                "Wolf",
            }
        },
        {
            "Arm_Super_Fist_1", new List<string>
            {
                "Bear",
                "Cyclops",
                "Golem",
                "Yeti",
            }
        },
        {
            "Arm_Super_Lighting_1", new List<string>
            {
                "Fire",
                "Lighting",
                "Robot",
                "Water",
            }
        },
        {
            "Arm_Super_Wind_1", new List<string>
            {
                "Mummy",
                "Skeleton",
                "Toad",
                "Walrus",
            }
        },

        {
            "Leg_Super_BruceLee_1", new List<string>
            {
                "Fire",
                "Ghost",
                "Skeleton",
                "Tree",
            }
        },
        {
            "Leg_Super_Cut_1", new List<string>
            {
                "Bear",
                "Crocodile",
                "Raven",
                "Wolf",
            }
        },
        {
            "Leg_Super_Dance_1", new List<string>
            {
                "Cyclops",
                "Mummy",
                "Scarecrow",
                "Yeti",
            }
        },
        {
            "Leg_Super_Jump_1", new List<string>
            {
                "Crab",
                "Scorpion",
                "Toad",
                "Walrus",
            }
        },
        {
            "Leg_Super_Wind_1", new List<string>
            {
                "Golem",
                "Lighting",
                "Robot",
                "Water",
            }
        },
    };

    // Use this for initialization
    void Start()
    {
        skinsList = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(true).Skins;

        tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.5f));
        tex.Apply();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnGUI()
    {
        TrackEntry entry = skeletonAnimation.state.GetCurrent(0);
        string animName = (entry == null ? null : entry.Animation.Name);
        string skinName = skeletonAnimation.skeleton.Skin.Name;
        string skinNamePrefix = skinName.Split('-')[0];
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle bgStyle = new GUIStyle();
        bgStyle.normal.background = tex;

        GUILayout.BeginHorizontal();

        #region animation_select

        // animation select
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, bgStyle, GUILayout.Width(buttonSize * 1.2f));
        GUILayout.Label("Select animation :");
        foreach (Spine.Animation a in skeletonAnimation.SkeletonDataAsset.GetSkeletonData(true).Animations)
        {
            if (a.Name.Equals("A"))
            {
                // skip this
                continue;
            }

            if (a.Name.StartsWith("Head_Super"))
            {
                // filter head super hits
                if (!a.Name.Contains(skinNamePrefix)) continue;
            }
            else if (a.Name.StartsWith("Arm_Super"))
            {
                // filter arms super hits
                if (superHitHashMap.ContainsKey(a.Name))
                {
                    if (!superHitHashMap[a.Name].Contains(skinsList.Items[armsSkin].Name))
                    {
                        continue;
                    }
                }
            }
            else if (a.Name.StartsWith("Leg_Super"))
            {
                // filter legs super hits
                if (superHitHashMap.ContainsKey(a.Name))
                {
                    if (!superHitHashMap[a.Name].Contains(skinsList.Items[legsSkin].Name))
                    {
                        continue;
                    }
                }
            }

            // create button for select animation
            buttonStyle.normal.textColor = a.Name.Equals(animName) ? Color.red : Color.gray;
            if (GUILayout.Button(a.Name, buttonStyle, GUILayout.Width(buttonSize),
                GUILayout.Height(buttonSize * 0.15f)))
            {
                if (string.Compare(a.Name, animName) != 0)
                {
                    skeletonAnimation.state.SetAnimation(0, a.Name, true);
                }
            }

            ;
        }

        GUILayout.EndScrollView();

        #endregion

        GUILayout.BeginArea(new Rect(Screen.width - buttonSize * 1.15f, 0, buttonSize * 1.15f, 600));

        GUILayout.BeginVertical(bgStyle);

        #region skin_select

        // skin select
        GUILayout.Label("Skin :");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(" < ", GUILayout.Width(buttonSize * 0.15f), GUILayout.Height(buttonSize * 0.15f)))
        {
            if (--currentSkin < 1) currentSkin = originalSkinsCount - 1; // 1 - skip "default" skin
            SafeSetSkin(skinsList.Items[currentSkin].Name);
        }

        ;
        GUILayout.Label(skinsList.Items[currentSkin].Name, labelStyle, GUILayout.Width(buttonSize * 0.75f),
            GUILayout.Height(buttonSize * 0.15f));
        if (GUILayout.Button(" > ", GUILayout.Width(buttonSize * 0.15f), GUILayout.Height(buttonSize * 0.15f)))
        {
            if (++currentSkin >= originalSkinsCount) currentSkin = 1; // 1 - skip "default" skin
            SafeSetSkin(skinsList.Items[currentSkin].Name);
        }

        GUILayout.EndHorizontal();

        #endregion

        #region mutations

        GUILayout.Label("Mutations :");
        MutatePart(ref headSkin, "Head");
        MutatePart(ref bodySkin, "Body");
        MutatePart(ref armsSkin, "Arms");
        MutatePart(ref legsSkin, "Legs");

        #endregion

        GUILayout.EndVertical();

        GUILayout.EndArea();

        GUILayout.EndHorizontal();
    }

    void MutatePart(ref int a_partIndex, string a_partName)
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(" < ", GUILayout.Width(buttonSize * 0.15f), GUILayout.Height(buttonSize * 0.15f)))
        {
            if (--a_partIndex < 1) a_partIndex = originalSkinsCount - 1; // 1 - skip "default" skin
            Mutate(a_partName, skinsList.Items[a_partIndex].Name);
        }

        ;
        if (currentSkin == a_partIndex)
        {
            GUILayout.Label("Change " + a_partName, labelStyle, GUILayout.Width(buttonSize * 0.75f),
                GUILayout.Height(buttonSize * 0.15f));
        }
        else
        {
            GUILayout.Label(a_partName + " : " + skinsList.Items[a_partIndex].Name, labelStyle,
                GUILayout.Width(buttonSize * 0.75f), GUILayout.Height(buttonSize * 0.15f));
        }

        if (GUILayout.Button(" > ", GUILayout.Width(buttonSize * 0.15f), GUILayout.Height(buttonSize * 0.15f)))
        {
            if (++a_partIndex >= originalSkinsCount) a_partIndex = 1; // 1 - skip "default" skin
            Mutate(a_partName, skinsList.Items[a_partIndex].Name);
        }

        ;
        GUILayout.EndHorizontal();
    }

    void SafeSetSkin(string skinName)
    {
        skeletonAnimation.Skeleton.SetSkin(skinName);
        if (mutantSkin != null)
        {
            skeletonAnimation.Skeleton.Data.Skins.Remove(mutantSkin);
            mutantSkin = null;
        }

        headSkin = bodySkin = armsSkin = legsSkin = currentSkin;

        TrackEntry entry = skeletonAnimation.state.GetCurrent(0);
        if (entry.Animation.Name.Contains("Super"))
        {
            // if super hit animation selected
            // select default animation (Stand_1)
            skeletonAnimation.state.SetAnimation(0, "Stand_1", true);
        }

        // reinit
        skeletonAnimation.initialSkinName = skinName;
        skeletonAnimation.Initialize(true);

        SafeUpdateAtlasRegionAttacher();
    }


    void Mutate(string monsterPartName, string skin2)
    {
        if (!partsHash.ContainsKey(monsterPartName))
        {
            Debug.Log("No monster part " + monsterPartName + " found");
            return;
        }

        List<string> monsterParts = partsHash[monsterPartName];
        if (mutantSkin == null)
        {
            mutantSkin = new Spine.Skin("Mutant");
            Spine.Skin oldSkin = skeletonAnimation.Skeleton.Skin;
            skeletonAnimation.Skeleton.Data.Skins.Add(mutantSkin);
            skeletonAnimation.Skeleton.SetSkin(mutantSkin);
            mutantSkin.CopySkin(oldSkin);
        }

        Spine.Skin skin = mutantSkin;

        for (int i = 0; i < monsterParts.Count; i++)
        {
            SetAttach(skeletonAnimation, skin, skin2, monsterParts[i]);
        }

        skeletonAnimation.Skeleton.SetSlotsToSetupPose();
        mutantSkin = skin;
        SafeUpdateAtlasRegionAttacher();
    }

    // set attaachment for slot 'a_slot' from skin name 'fromSkin' to skin 'a_skin'
    public static void SetAttach(SkeletonAnimation a_skeleton, Spine.Skin a_skin, string fromSkin, string a_slot)
    {
        Spine.Slot slot = a_skeleton.skeleton.FindSlot(a_slot);
        int index = a_skeleton.skeleton.FindSlotIndex(a_slot);
        Spine.Attachment att = GetAttachFromSkin(a_skeleton, fromSkin, a_slot);
        Spine.RegionAttachment newRegAtt = att as Spine.RegionAttachment;

        if (slot != null)
        {
            if (newRegAtt != null)
            {
                a_skin.SetAttachment(index, a_slot, newRegAtt);
            }
            else
            {
                a_skin.RemoveAttachment(index, a_slot);
            }

            slot.Attachment = newRegAtt;
        }
    }

    static Attachment GetAttachFromSkin(SkeletonAnimation a_skeleton, string fromSkin, string slot)
    {
        Skin skin = a_skeleton.skeleton.Data.FindSkin(fromSkin);
        if (skin != null)
        {
            int index = a_skeleton.skeleton.FindSlotIndex(slot);
            return skin.GetAttachment(index, slot);
        }

        Debug.LogError("Skeleton " + a_skeleton.skeletonDataAsset.name + " Skin not found " + fromSkin + " for slot " +
                       slot);
        return null;
    }

    void SafeUpdateAtlasRegionAttacher()
    {
        if (demoAtlasRegionAttacher != null)
        {
//			demoAtlasRegionAttacher.OnSkinChanged();
        }

        if (bottleAtlasRegionAttacher != null)
        {
//			bottleAtlasRegionAttacher.OnSkinChanged();
        }

//		skeletonAnimation.Skeleton.SetSlotsToSetupPose();
    }
}