using UnityEngine;

public class DemoAtlasRegionAttacher : BaseAtlasRegionAttacher {
	private string[] _armorSkins = {
		"Barbed_",
		"Bone_",
		"Conch_",
		"Deluxe_",
		"Eagle_",
		"Gentleman_",
		"Glamor_",
		"Gold_",
		"Jester_",
		"King_",
		"Leather_",
		"Magister_",
		"Mexican_",
		"Persian_",
		"Pike_",
		"Pirate_",
		"Roman_",
		"Shaman_",
		"Sheriff_",
		"Soldier_",
		"Stargazer_",
		"Steel_",
		"Terrorist_",
		"Tiger_",
		"Warlock_",
		"Winter_",
		"Wood_"
	};
	string shoulder;

	public override void Awake() {
		_skins = _armorSkins;
		base.Awake();
	}

	private void OnGUI() {
		bool dirty = false;
		GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
		labelStyle.alignment = TextAnchor.MiddleCenter;

		GUIStyle bgStyle = new GUIStyle();
		bgStyle.normal.background = tex;
		GUILayout.BeginArea(new Rect(Screen.width - buttonSize * 1.15f, 200f, buttonSize * 1.15f, 500));

		// skin select
		GUILayout.BeginVertical(bgStyle);
		GUILayout.Label("Armor : "  );
		foreach (var entry in attachments) {
			if (entry.slot.Equals("shoulder_R")) {
				entry.region = shoulder + "_R";
				continue;
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("< ", GUILayout.Width(buttonSize * 0.15f), GUILayout.Height(buttonSize * 0.15f))) {
				entry.region = PrevSkin(entry.region.Replace(entry.slot, "")) + entry.slot;
				dirty = true;
			};
			GUILayout.Label(entry.region, labelStyle, GUILayout.Width(buttonSize * 0.75f), GUILayout.Height(buttonSize * 0.15f));
			if (GUILayout.Button(" >", GUILayout.Width(buttonSize * 0.15f), GUILayout.Height(buttonSize * 0.15f))) {
				entry.region = NextSkin(entry.region.Replace(entry.slot, "")) + entry.slot;
				dirty = true;
			};
			GUILayout.EndHorizontal();
			if (entry.slot.Equals("shoulder_L")) {
				shoulder = entry.region.Replace("_L", "");
			}
		}
		GUILayout.EndVertical();

		GUILayout.EndArea();

		if (dirty) OnSkinChanged();
	}
	
	private string NextSkin(string a_skin) {
		var index = System.Array.IndexOf(_skins, a_skin);
		index = ++index % _skins.Length;
		return _skins[index];
	}

	private string PrevSkin(string a_skin) {
		var index = System.Array.IndexOf(_skins, a_skin);
		if (--index < 0) index = _skins.Length - 1;
		return _skins[index];
	}
}
