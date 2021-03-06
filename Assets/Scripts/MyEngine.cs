using System.Collections.Generic;
using Leopotam.Ecs;
using Source.Systems;
using Systems;
using UnityEngine;
using UnityEngine.UI;
using Voody.UniLeo;
using Random = System.Random;

namespace Scripts
{
    public class MyEngine : MonoBehaviour
    {
        public static readonly Random random = new Random();
        public EcsWorld world { get; private set; } = null!;
        private EcsSystems systems = null!;

        public GameObject nodePiece = null!;
        [HideInInspector]public List<Sprite> sprites = null!;

        [HideInInspector]public int width;
        [HideInInspector]public int height;
        
        public Text score = null!;
        public RectTransform gameBoard = null!;
        public RectTransform fallBoard = null!;

        public EcsEntity?[,] board = null!;

        public float pixelPerMeterX { get; private set; }
        public float pixelPerMeterY { get; private set; }

        public StartPieces startLevel = null!;

        private void Start()
        {
            var layout = startLevel;
            width = layout.list[0].images.Count;
            height = layout.list.Count;
            
            Screen.fullScreen = true;
            world = new EcsWorld();
            systems = new EcsSystems(world);

            systems.ConvertScene();

            AddInjections();
            AddOneFrames();
            AddSystems();

            systems.Init();

            new BoardInitializer(this);
        }

        private void AddInjections()
        {
        }

        private void AddSystems()
        {
            systems.Add(new DragPieceSystem());
            systems.Add(new DropPieceSystem(this));
            systems.Add(new MovePieceSystem(this));
            systems.Add(new AwaitPairSystem());
            systems.Add(new MatchSystem(this));
            systems.Add(new DestroySystem(this));
            systems.Add(new RollbackSystem(this));
            systems.Add(new FallSystem(this));
        }

        private void AddOneFrames()
        {
            systems.OneFrame<DropPieceEvent>();
        }

        private void Update()
        {
            pixelPerMeterX = gameBoard.rect.width / width;
            pixelPerMeterY = gameBoard.rect.height / height;

            systems.Run();

            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }

        private void OnDestroy()
        {
            systems.Destroy();
            world.Destroy();
        }
    }
}