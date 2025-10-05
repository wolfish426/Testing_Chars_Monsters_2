using Spine.Unity;
using UnityEngine;

public class BottleAtlasRegionAttacher : BaseAtlasRegionAttacher {
	private string[] _bottlesSkins = {
		"Potion_black_s",
		"Potion_black",
		"Potion_blue_s",
		"Potion_blue",
		"Potion_fiolet_s",
		"Potion_fiolet",
		"Potion_green_s",
		"Potion_green",
		"Potion_grey_s",
		"Potion_grey",
		"Potion_orange_s",
		"Potion_orange",
		"Potion_red_s",
		"Potion_red",
		"Potion_siniy_s",
		"Potion_siniy",
		"Potion_white_s",
		"Potion_white",
		"Potion_yellow_s",
		"Potion_yellow"
	};

	private int _bottleIndex = 0;

	public override void Awake() {
		_skins = _bottlesSkins;
	//	base.Awake();
	}

	private void OnGUI() {
		var skeletonAnimation = GetComponent<SkeletonAnimation>();
		var trackEntry = skeletonAnimation.AnimationState.GetCurrent(0);
		if (trackEntry != null)
		{
			if (string.Compare(trackEntry.Animation.Name, "Drink_1") != 0)
			{
				return;
			}
		}

		bool dirty = false;
		GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
		labelStyle.alignment = TextAnchor.MiddleCenter;

		GUIStyle bgStyle = new GUIStyle();
		bgStyle.normal.background = tex;
		GUILayout.BeginArea(new Rect(Screen.width - buttonSize * 1.15f, 350f, buttonSize * 1.15f, 500));

		// skin select
		GUILayout.BeginVertical(bgStyle);
		GUILayout.Label("Bottle for drink :");
		foreach (var entry in attachments) {
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("< ", GUILayout.Width(buttonSize * 0.15f), GUILayout.Height(buttonSize * 0.15f))) {
				entry.region = PrevSkin();
				dirty = true;
			};
			GUILayout.Label(entry.region, labelStyle, GUILayout.Width(buttonSize * 0.75f), GUILayout.Height(buttonSize * 0.15f));
			if (GUILayout.Button(" >", GUILayout.Width(buttonSize * 0.15f), GUILayout.Height(buttonSize * 0.15f))) {
				entry.region = NextSkin();
				dirty = true;
			};
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();

		GUILayout.EndArea();

		if (dirty) OnSkinChanged();
	}
	
	private string NextSkin() {
		_bottleIndex = ++_bottleIndex % _skins.Length;
		return _skins[_bottleIndex];
	}

	private string PrevSkin() {
		if (--_bottleIndex < 0) _bottleIndex = _skins.Length - 1;
		return _skins[_bottleIndex];
	}
}
