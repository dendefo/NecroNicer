using UnityEngine;
using static Face.FaceStats;

public class DiceControl : MonoBehaviour
{
    public Rigidbody body;
    public bool Dragable = true;
    public Vector3 startPosition;
    public UnitScript Unit;
    public DiceFace[] Faces;

    bool inMove { get; set; } = false;
    private Vector3 delta { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        delta = Input.mousePosition;
        startPosition = transform.localPosition;
    }

    void Update()
    {
        MouseRotation();
    }

    void MouseRotation()
    {
        delta -= Input.mousePosition;
        if (Input.GetMouseButtonDown(1)) inMove = true;
        if (Input.GetMouseButtonUp(1)) inMove = false;
        if (inMove && !body.useGravity) body.AddTorque(new Vector3(-delta.y, delta.x, 0), ForceMode.Acceleration);
        delta = Input.mousePosition;
    }


    private void OnMouseDown()
    {

        try
        {
            if (((BattleSceneManager)BattleSceneManager.Instance).inBattle)
            {
                ((BattleSceneManager)BattleSceneManager.Instance).ThrowTheDice(this);
                return;
            }
        }
        catch { }
        if (!Dragable) return;
    }
    private void OnMouseDrag()
    {

        if (!Dragable) return;
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x, pos.y);
    }
    private void OnMouseUp()
    {
        if (!Dragable) return;
        bool changed = false;
        foreach (UnitScript Unit in GameManager.Instance.PlayersUnits)
        {
            float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y),
                new Vector2(Unit.transform.position.x, Unit.transform.position.y));
            if (distance < 0.4 && distance > 0.25 && Unit.UnitsDice == null
                && transform.position.y < Unit.transform.position.y
                && transform.position.x < Unit.transform.position.x + 0.1
                && transform.position.x > Unit.transform.position.x - 0.1)
            {
                if (this.Unit != null && this.Unit != Unit)
                {
                    this.Unit.spriteRend.gameObject.SetActive(true);
                    Destroy(this.Unit.Rig);
                    this.Unit.GetComponent<UnitScript>().UnitsDice = null;
                    this.Unit.transform.eulerAngles = new();
                }
                changed = true;
                transform.parent = Unit.transform;
                Unit.UnitsDice = gameObject;
                transform.localPosition = new Vector3(0, -0.80f, 0);
                startPosition = transform.localPosition;
                this.Unit = Unit;
                GetComponent<MeshCollider>().enabled = false;
                Unit.spriteRend.gameObject.SetActive(false);
                var un = Instantiate(Unit.EnemyScriptableObjects.RiggedPrefab, transform.parent);
                Unit.Rig = un;
                Unit.UnitAnimator = un.GetComponent<Animator>();
                Unit.UnitStats = Unit.EnemyScriptableObjects.BasicStats;
                if (!Unit.IsEnemy)
                {
                    Unit.gameObject.transform.eulerAngles = new(0, 180, 0);
                    un.transform.localScale = Vector3.one*1.5f;
                }

            }
        }
        if (!changed) transform.localPosition = startPosition;
        else
        {
            int i = 0;

            foreach (UnitScript Unit in GameManager.Instance.PlayersUnits)
            {
                if (Unit.UnitsDice != null) i++;
            }
            if (i == GameManager.Instance.PlayersUnits.Count) GameManager.Instance.DicePanelParent.SetActive(false);
        }
    }
    public void Generate(FaceChance[] faceChance)
    {

        int sum = 0;
        foreach (var chance in faceChance)
        {
            for (int i = 0; i < chance.Amount; i++)
            {
                TierTypes type = chance.Chances[PickRandomFace(chance)].ChanceTierTypes;
                var material = GameManager.Instance.MainLibrary.GetFace(chance.abilityTypeFace, type);
                var face = new InstanceFace();
                face.faceDefenition = material;
                face.Uses = material.Stats.MaxUses;
                Faces[sum].Face = face;
                sum++;

            }

        }

    }
    private int PickRandomFace(FaceChance faceChances)
    {
        double chancesSumm = 0;
        float rand = Random.Range(0, 1.0f);
        for (int i = 0; i < faceChances.Chances.Length; i++)
        {
            chancesSumm += faceChances.Chances[i].Chances;
            if (chancesSumm >= rand) return i;
        }

        return faceChances.Chances.Length;
    }
}
