using UnityEngine;
using System.Collections;

public class Cell : BaseObj {
	public enum cellType {Empty, Player, Canon, Building};
	public cellType type;
	public bool selected;
	public Liveable liveObj;
	public int costExpend=20;

	//colors
	public Color ColorSelected = Color.red;
	public Color ColorBase = Color.white;
	Vector2[] distanceEdges = new Vector2[] {new Vector2(3.8f,2.15f), new Vector2(3.8f,-2.2f), new Vector2(0,-4.35f), new Vector2(-3.8f,-2.2f), new Vector2(-3.8f,2.15f), new Vector2(0,4.35f)};
	//Vector2[] distanceEdges = new Vector2[] {new Vector2(3.8f,2.15f)};


	SpriteRenderer sprite;
	//events
	public delegate void HitEvent(object sender, object args); public HitEvent OnHit;

	protected new void Start(){
		base.Start();
		sprite = GetComponent<SpriteRenderer>();
		Game.cells.Add(this);
	}

	protected new void Update(){
		base.Update();
	}
	
	void OnDestroy(){
		Game.cells.Remove(this);
	}

	public void select(){
		var selected = getSelected();
		if (selected != null) selected.unSelect();
		sprite.color = ColorSelected;
		Game.menue.GetComponent<RectTransform>().position = new Vector2(transform.position.x + 2, transform.position.y + 2);
	}

	public void unSelect(){	
		sprite.color = ColorBase;
		renderer.material.color = ColorBase;
	}
	
	public bool isSelected(){
		return sprite.color == ColorSelected;
	}

	public static Cell getSelected(){
		foreach (var c in GetGame().cells)
			if (c.isSelected()) return c;
		return null;
	}

	public void expend(){
		Game.minerals -= costExpend;
		unSelect();
		foreach (var v2 in distanceEdges){
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(200, 200, 0));
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
			var hit = Physics2D.Raycast(ray.origin, ray.direction ,Mathf.Infinity , ( 1 << LayerMask.NameToLayer("Cells") ));
			//var hit = Physics2D.Raycast(Camera.main.transform.position, gameObject.transform.position +  new Vector3(v2.x,v2.y,1) ,101 , ( 1 << LayerMask.NameToLayer("Cells") ));
			if (hit.collider == null){
				GameObject.Instantiate(gameObject, transform.position +  new Vector3(v2.x,v2.y,0),transform.rotation);
			} else {
				l ("hit " + ((Vector2)transform.position +  v2)+ " | " + hit.collider.gameObject.name);
			}
			Debug.DrawLine(Camera.main.transform.position, gameObject.transform.position +  new Vector3(v2.x,v2.y,1), Color.yellow, Mathf.Infinity);
		}
	}

	public bool canExpend(){
		return (Game.minerals >= costExpend);
	}

	public void HandleSelect(Drag.DragType dragType, Building building){
		switch (dragType) {
		case Drag.DragType.Build:				
			if (!liveObj && Game.canBuild(building)) select();
			break;
		case Drag.DragType.Upgrade:
			if (liveObj && Game.canUpgrade(building)) select();
			break;
		case Drag.DragType.Refund:
			if (liveObj) select();
			break;
		case Drag.DragType.Expand:
			if (canExpend()) select();
			break;
			default: break;
		}
	}
}
