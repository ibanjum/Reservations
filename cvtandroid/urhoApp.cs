using System.Diagnostics;
using System.Linq;
using Urho;

namespace cvtandroid
{
    public class urhoApp : Application
    {
        Node rootNode;
        Node cameraNode;
        Camera camera;
        Scene scene;
        float min = 0;
        float max = 0;
        bool scaling;
        float TouchSensitivity = 3;
        float Yaw;
        float Pitch;

        [Preserve]
        public urhoApp(ApplicationOptions options = null) : base(options) { }

        protected override void Start()
        {
            base.Start();
            CreateScene();
        }

        static urhoApp()
        {
            UnhandledException += (s, e) =>
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                e.Handled = true;
            };
        }
        void CreateScene()
        {

            scene = new Scene();
            scene.CreateComponent<Octree>();
            rootNode = scene.CreateChild("rootNode");
            rootNode.SetScale(1f);

            Constructivity.Core.Product product = null;
            Constructivity.Core.Mesh m = null;
            if (Xamarin.Forms.Application.Current.Properties.ContainsKey("Meshes"))
            {
                if(Xamarin.Forms.Application.Current.Properties["Meshes"].GetType() == typeof(Constructivity.Core.Product))
                {
                    product = Xamarin.Forms.Application.Current.Properties["Meshes"] as Constructivity.Core.Product;
                    ConvertAllMeshes(product.Meshes);
                }
                else if(Xamarin.Forms.Application.Current.Properties["Meshes"].GetType() == typeof(Constructivity.Core.Mesh))
                {
                    m = Xamarin.Forms.Application.Current.Properties["Meshes"] as Constructivity.Core.Mesh;
                    ConvertSingleMesh(m);
                }
            }

            Node lightNode = scene.CreateChild();
            lightNode.SetDirection(new Vector3(0f, 0f, -5f));
            Light light = lightNode.CreateComponent<Light>();
            light.Brightness = 1.1f;

            // Create the camera
            cameraNode = scene.CreateChild(name: "camera");
            camera = cameraNode.CreateComponent<Camera>();
            cameraNode.Position = new Vector3(0, 0, -10);

            var renderer = Renderer;
            var port = new Viewport(Context, scene, camera, null);
            renderer.SetViewport(0, port);

            port.SetClearColor(Color.White);
        }
        protected void ConvertAllMeshes(Constructivity.Core.Mesh[] meshes)
        {
            foreach (Constructivity.Core.Mesh meshRaw in meshes)
            {
                Constructivity.Core.Mesh mesh = Tessellation.Tesselate(meshRaw);
                uint numVertices = (uint)(mesh.Vertices.Length / 6);

                float[] vertexData = new float[mesh.Vertices.Length];
                for (int i = 0; i < mesh.Vertices.Length; i++)
                {
                    vertexData[i] = (float)mesh.Vertices[i];
                    if (vertexData[i] > max)
                    {
                        max = vertexData[i];
                    }
                    if (vertexData[i] < min)
                    {
                        min = vertexData[i];
                    }
                }

                uint numIndices = (uint)mesh.Faces.Length;
                short[] indexData = new short[mesh.Faces.Length];
                for (int i = 0; i < mesh.Faces.Length; i++)
                {
                    indexData[i] = (short)mesh.Faces[i];
                }

                Geometry geom = new Geometry();
                VertexBuffer vb = new VertexBuffer(Context, false);
                IndexBuffer ib = new IndexBuffer(Context, false);

                vb.Shadowed = true;
                vb.SetSize(numVertices, ElementMask.Position | ElementMask.Normal, false);
                vb.SetData(vertexData.ToArray());

                ib.Shadowed = true;
                ib.SetSize(numIndices, false, false);
                ib.SetData(indexData.ToArray());

                geom.SetVertexBuffer(0, vb);
                geom.IndexBuffer = ib;
                geom.SetDrawRange(PrimitiveType.TriangleList, 0, numIndices, true);

                Model fromScratchModel = new Model();
                fromScratchModel.NumGeometries = 1;
                fromScratchModel.SetGeometry(0, 0, geom);
                fromScratchModel.BoundingBox = new BoundingBox(new Vector3(min, min, min), new Vector3(max, max, max));

                Node node = rootNode.CreateChild();
                StaticModel sm = node.CreateComponent<StaticModel>();
                sm.Model = fromScratchModel;

                if (mesh.Material != null)
                {
                    System.Drawing.Color myColor = System.Drawing.Color.FromArgb((int)mesh.Material.Color);
                    Color urhoColor = ConvertColor(myColor);
                    Material material = Material.FromColor(urhoColor);
                    material.SetTechnique(0, CoreAssets.Techniques.NoTexture);
                    sm.SetMaterial(material);
                }
            }
        }
        protected void ConvertSingleMesh(Constructivity.Core.Mesh meshRaw)
        {
            Constructivity.Core.Mesh mesh = Tessellation.Tesselate(meshRaw);
            uint numVertices = (uint)(mesh.Vertices.Length / 6);

            float[] vertexData = new float[mesh.Vertices.Length];
            for (int i = 0; i < mesh.Vertices.Length; i++)
            {
                vertexData[i] = (float)mesh.Vertices[i];
                if (vertexData[i] > max)
                {
                    max = vertexData[i];
                }
                if (vertexData[i] < min)
                {
                    min = vertexData[i];
                }
            }

            uint numIndices = (uint)mesh.Faces.Length;
            short[] indexData = new short[mesh.Faces.Length];
            for (int i = 0; i < mesh.Faces.Length; i++)
            {
                indexData[i] = (short)mesh.Faces[i];
            }

            Geometry geom = new Geometry();
            VertexBuffer vb = new VertexBuffer(Context, false);
            IndexBuffer ib = new IndexBuffer(Context, false);

            vb.Shadowed = true;
            vb.SetSize(numVertices, ElementMask.Position | ElementMask.Normal, false);
            vb.SetData(vertexData.ToArray());

            ib.Shadowed = true;
            ib.SetSize(numIndices, false, false);
            ib.SetData(indexData.ToArray());

            geom.SetVertexBuffer(0, vb);
            geom.IndexBuffer = ib;
            geom.SetDrawRange(PrimitiveType.TriangleList, 0, numIndices, true);

            Model fromScratchModel = new Model();
            fromScratchModel.NumGeometries = 1;
            fromScratchModel.SetGeometry(0, 0, geom);
            fromScratchModel.BoundingBox = new BoundingBox(new Vector3(min, min, min), new Vector3(max, max, max));

            Node node = rootNode.CreateChild();
            StaticModel sm = node.CreateComponent<StaticModel>();
            sm.Model = fromScratchModel;

            if (mesh.Material != null)
            {
                System.Drawing.Color myColor = System.Drawing.Color.FromArgb((int)mesh.Material.Color);
                Color urhoColor = ConvertColor(myColor);
                Material material = Material.FromColor(urhoColor);
                material.SetTechnique(0, CoreAssets.Techniques.NoTexture);
                sm.SetMaterial(material);
            }
        }
        void OnTouchBegin(TouchBeginEventArgs e)
        {
            scaling = false;
        }
        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);
            PinchGesture(timeStep);
            rotateNode(timeStep);
        }
        private void rotateNode(float timeStep)
        {
            var input = Input;
            for (uint i = 0, num = input.NumTouches; i < num; ++i)
            {
                TouchState state = input.GetTouch(i);
                if (state.Delta.X != 0 || state.Delta.Y != 0)
                {
                    Yaw += TouchSensitivity * camera.Fov / Graphics.Height * state.Delta.X;
                    Pitch += TouchSensitivity * camera.Fov / Graphics.Height * state.Delta.Y;
                    rootNode.Rotation = new Quaternion(Yaw, Pitch, 0);
                }
            }
        }
        public void PinchGesture(float timeStep)
        {
            if (Input.NumTouches == 2)
            {
                scaling = true;
                var state1 = Input.GetTouch(0);
                var state2 = Input.GetTouch(1);
                var distance1 = IntVector2.Distance(state1.Position, state2.Position);
                var distance2 = IntVector2.Distance(state1.LastPosition, state2.LastPosition);
                rootNode.SetScale(rootNode.Scale.X + (distance1 - distance2) / 5000f);
            }
        }
        public static Color ConvertColor(System.Drawing.Color color)
        {
            return new Color(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 1f);
        }
    }
}
