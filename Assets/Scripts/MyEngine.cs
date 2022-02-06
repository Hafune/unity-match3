using Leopotam.Ecs;
using Source;
using Source.Components;
using Source.Systems;
using UnityEngine;
using Voody.UniLeo;

public class MyEngine : MonoBehaviour
{
    public EcsWorld world { get; private set; } = null!;
    private EcsSystems systems = null!;

    public GameObject nodePiece = null!;
    public Sprite[] sprites = null!;

    public RectTransform gameBoard = null!;
    public BoardInitializer boardInitializer = null!;

    public int?[,] valuesBoard = null!;

    public float pixelPerMeter { get; private set; }

    private void Start()
    {
        Screen.fullScreen = true;
        world = new EcsWorld();
        systems = new EcsSystems(world);

        systems.ConvertScene();

        AddInjections();
        AddOneFrames();
        AddSystems();

        systems.Init();

        boardInitializer = new BoardInitializer(this);
    }

    private void AddInjections()
    {
    }

    private void AddSystems()
    {
        systems.Add(new MatchSystem(this));
        systems.Add(new DestroySystem(this));
        systems.Add(new DropPieceSystem());
        systems.Add(new MovePieceSystem(this));
        systems.Add(new RollbackSystem(this));
        systems.Add(new FallSystem(this));
    }

    private void AddOneFrames()
    {
        systems.OneFrame<DropPieceEvent>();
    }

    private void Update()
    {
        pixelPerMeter = gameBoard.rect.width / boardInitializer.width;

        systems.Run();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Quit the application
            Application.Quit();
        }
    }

    private void OnDestroy()
    {
        systems.Destroy();
        world.Destroy();
    }
}