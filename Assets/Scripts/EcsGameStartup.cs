using Leopotam.Ecs;
using Source;
using Source.Systems;
using UnityEngine;
using Voody.UniLeo;

public class EcsGameStartup : MonoBehaviour
{
    private EcsWorld world = null!;
    private EcsSystems systems = null!;

    public GameObject nodePiece = null!;
    public Sprite[] sprites = null!;

    public RectTransform gameBoard = null!;
    public BoardInitializer boardInitializer = null!;

    private void Start()
    {
        world = new EcsWorld();
        systems = new EcsSystems(world);

        systems.ConvertScene();

        AddInjections();
        AddOneFrames();
        AddSystems();

        systems.Init();

        boardInitializer = new BoardInitializer(world, sprites, nodePiece, gameBoard);
    }

    private void AddInjections()
    {
    }

    private void AddSystems()
    {
        systems.Add(new MovementSystem(this));
    }

    private void AddOneFrames()
    {
    }

    private void Update()
    {
        systems.Run();
    }

    private void OnDestroy()
    {
        systems.Destroy();
        world.Destroy();
    }
}