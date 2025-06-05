using System.Windows.Controls;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using SCARA_ROBOT_SOFTWARE_WPF.ViewModels;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Input;

namespace SCARA_ROBOT_SOFTWARE_WPF.Views
{
    public partial class SetupProgramView : UserControl
    {
        private readonly UartManager _uartManager = UartManager.Instance;
        private List<ModelVisual3D> cornerMarkers = new List<ModelVisual3D>();
        private List<BillboardTextVisual3D> cornerLabels = new List<BillboardTextVisual3D>();

        public SetupProgramView(SetupProgramViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            DrawCartesianRobot3D();
            RobotViewport.MouseDown += RobotViewport_MouseDown;
            vm.PropertyChanged += Vm_PropertyChanged;
            SetSelectedCorner(vm.SelectedCornerIndex);

        }
        private void RobotViewport_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(RobotViewport);
            var hits = RobotViewport.Viewport.FindHits(mousePos);

            if (hits != null && hits.Count > 0)
            {
                for (int i = 0; i < cornerMarkers.Count; i++)
                {
                    if (hits[0].Visual == cornerMarkers[i])
                    {
                        if (DataContext is SetupProgramViewModel vm)
                        {
                            vm.SelectedCornerIndex = i;
                        }
                        break;
                    }
                }
            }
        }

        private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SetupProgramViewModel.SelectedCornerIndex))
            {
                if (DataContext is SetupProgramViewModel vm)
                    SetSelectedCorner(vm.SelectedCornerIndex);
            }
        }

        public void SetSelectedCorner(int selectedIndex)
        {
            for (int i = 0; i < cornerMarkers.Count; i++)
            {
                var modelVisual = cornerMarkers[i];
                var geometryModel = modelVisual.Content as GeometryModel3D;
                if (geometryModel != null)
                {
                    var color = (i == selectedIndex) ? Colors.LimeGreen : Colors.Red;
                    geometryModel.Material = MaterialHelper.CreateMaterial(color);
                }
            }
        }

        private void AddCorner(Point3D pos, string label, Color color)
        {
            var markerMesh = new MeshBuilder();
            markerMesh.AddSphere(pos, 1.2);  
            var markerModel = new GeometryModel3D
            {
                Geometry = markerMesh.ToMesh(),
                Material = MaterialHelper.CreateMaterial(color)
            };
            var marker = new ModelVisual3D { Content = markerModel };
            RobotViewport.Children.Add(marker);
            cornerMarkers.Add(marker);

            var text = new BillboardTextVisual3D
            {
                Text = label,
                Position = new Point3D(pos.X, pos.Y + 4, pos.Z), 
                Foreground = Brushes.Red,
                FontSize = 24
            };
            RobotViewport.Children.Add(text);
            cornerLabels.Add(text);
        }
        private void DrawCartesianRobot3D()
        {
            RobotViewport.Children.Clear();

            double groundWidth = 80;
            double groundDepth = 80;
            double groundY = 0;

            double baseWidth = 20;
            double baseHeight = 4;
            double baseDepth = 16;

            double offsetX = -groundWidth / 2 + baseWidth / 2;    
            double offsetZ = -groundDepth / 2 + baseDepth / 2;   
            double offsetY = baseHeight / 2;               


            var groundMesh = new MeshBuilder(false, false);
            groundMesh.AddBox(new Point3D(0, 0, 0), groundWidth, 1, groundDepth);
            var groundModel = new GeometryModel3D
            {
                Geometry = groundMesh.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.Black, 0.25)
            };
            RobotViewport.Children.Add(new ModelVisual3D { Content = groundModel });

            double halfW = groundWidth / 2, halfD = groundDepth / 2;

            AddCorner(new Point3D(-halfW + 7, groundY + 2, -halfD + 50), "A", Colors.Red); 
            AddCorner(new Point3D(-halfW + 50, groundY + 2, halfD + 0), "B", Colors.Red);  
            AddCorner(new Point3D(halfW - 0, groundY + 2, halfD - 35), "C", Colors.Red);   
            AddCorner(new Point3D(halfW - 35, groundY + 2, -halfD + 5), "D", Colors.Red); 

            var baseMesh = new MeshBuilder(false, false);
            baseMesh.AddBox(new Point3D(offsetX, offsetY, offsetZ), baseWidth, baseHeight, baseDepth);
            var baseModel = new GeometryModel3D
            {
                Geometry = baseMesh.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.DarkGray)
            };
            RobotViewport.Children.Add(new ModelVisual3D { Content = baseModel });

            double columnHeight = 44;
            double columnY = offsetY + columnHeight / 2 + baseHeight / 2;
            var columnMesh = new MeshBuilder(false, false);
            columnMesh.AddBox(new Point3D(offsetX, columnY, offsetZ), 4, columnHeight, 4);
            var columnModel = new GeometryModel3D
            {
                Geometry = columnMesh.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.SteelBlue)
            };
            RobotViewport.Children.Add(new ModelVisual3D { Content = columnModel });

            double beamLength = 36;
            double beamSizeY = 3.0;
            double beamSizeZ = 3.5;
            double beamY = columnY; 
            double beamXStart = offsetX; 

            var beamXMesh = new MeshBuilder(false, false);
            beamXMesh.AddBox(new Point3D(beamXStart + beamLength / 2, beamY, offsetZ), beamLength, beamSizeY, beamSizeZ);
            var beamXModel = new GeometryModel3D
            {
                Geometry = beamXMesh.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.DodgerBlue)
            };
            RobotViewport.Children.Add(new ModelVisual3D { Content = beamXModel });

            double carriageSize = 3;
            double carriageX = beamXStart + 25;
            double carriageY = beamY - beamSizeY / 2 - 1;
            var carriageMesh = new MeshBuilder(false, false);
            carriageMesh.AddBox(new Point3D(carriageX, carriageY, offsetZ), carriageSize, 2, 3);
            var carriageModel = new GeometryModel3D
            {
                Geometry = carriageMesh.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.Red)
            };
            RobotViewport.Children.Add(new ModelVisual3D { Content = carriageModel });

            double subBeamY = carriageY - 1.6;
            double subBeamXStart = carriageX - carriageSize / 2 + 1.5;
            double subBeamLength = 13;
            var subBeamMesh = new MeshBuilder(false, false);
            subBeamMesh.AddBox(
                new Point3D(subBeamXStart + subBeamLength / 2, subBeamY, offsetZ),
                subBeamLength, 1.2, 2.2
            );
            var subBeamModel = new GeometryModel3D
            {
                Geometry = subBeamMesh.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.LimeGreen)
            };
            RobotViewport.Children.Add(new ModelVisual3D { Content = subBeamModel });

            double endBlockSize = 2.5;
            double endBlockX = subBeamXStart + subBeamLength + endBlockSize / 2;
            double endBlockY = subBeamY;
            var endBlockMesh = new MeshBuilder(false, false);
            endBlockMesh.AddBox(new Point3D(endBlockX, endBlockY, offsetZ), endBlockSize, endBlockSize, endBlockSize);
            var endBlockModel = new GeometryModel3D
            {
                Geometry = endBlockMesh.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.Gold)
            };
            RobotViewport.Children.Add(new ModelVisual3D { Content = endBlockModel });

            double fingerLength = 2.0;
            double fingerWidth = 0.4;
            double fingerHeight = 0.4;
            double gapZ = 0.7;

            double fingerX = endBlockX + endBlockSize / 2 + fingerLength / 2; 
            double fingerY = endBlockY; 

            double fingerZ1 = offsetZ - gapZ;
            double fingerZ2 = offsetZ + gapZ;

            var fingerLeft = new MeshBuilder(false, false);
            fingerLeft.AddBox(new Point3D(fingerX, fingerY, fingerZ1), fingerLength, fingerHeight, fingerWidth);
            var fingerLeftModel = new GeometryModel3D
            {
                Geometry = fingerLeft.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.Yellow)
            };
            RobotViewport.Children.Add(new ModelVisual3D { Content = fingerLeftModel });

            var fingerRight = new MeshBuilder(false, false);
            fingerRight.AddBox(new Point3D(fingerX, fingerY, fingerZ2), fingerLength, fingerHeight, fingerWidth);
            var fingerRightModel = new GeometryModel3D
            {
                Geometry = fingerRight.ToMesh(),
                Material = MaterialHelper.CreateMaterial(Colors.Yellow)
            };
            RobotViewport.Children.Add(new ModelVisual3D { Content = fingerRightModel });

            RobotViewport.Children.Add(new DefaultLights());

            Dispatcher.BeginInvoke(new Action(() =>
            {
                RobotViewport.Camera = new PerspectiveCamera
                {
                    Position = new Point3D(190.47900894097248, 130.32753628355005, 200.6912391075898),
                    LookDirection = new Vector3D(-216.8947907638671, -128.73687445832033, -222.8947907638671),
                    UpDirection = new Vector3D(0, 1, 0),
                    FieldOfView = 30
                };
            }), System.Windows.Threading.DispatcherPriority.Background);

            RobotViewport.CameraChanged += (s, e) =>
            {
                var cam = RobotViewport.Camera as PerspectiveCamera;
                if (cam != null)
                {
                    Debug.WriteLine($"Camera: Position=({cam.Position.X}, {cam.Position.Y}, {cam.Position.Z}), " +
                                    $"LookDirection=({cam.LookDirection.X}, {cam.LookDirection.Y}, {cam.LookDirection.Z}), " +
                                    $"FieldOfView={cam.FieldOfView}");
                }
            };
        }

        private void btnYUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StartHold("PyUp");
        }
        private void btnYUp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StopHold();
        }
        private void btnYDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StartHold("PyDown");
        }
        private void btnYDown_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StopHold();
        }

        private void btnXLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StartHold("PxLeft");
        }
        private void btnXLeft_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StopHold();
        }
        private void btnXRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StartHold("PxRight");
        }
        private void btnXRight_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StopHold();
        }
        private void btnZUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StartHold("PzUp");
        }
        private void btnZUp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StopHold();
        }
        private void btnZDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StartHold("PzDown");
        }
        private void btnZDown_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StopHold();
        }
        private void btnCapLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StartHold("YawLeft");
        }
        private void btnCapLeft_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StopHold();
        }
        private void btnCapRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StartHold("YawRight");
        }
        private void btnCapRight_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as SetupProgramViewModel)?.StopHold();
        }

        private void PointA_Click(object sender, RoutedEventArgs e)
        {
            MoveToPoint("A");
        }

        private void PointB_Click(object sender, RoutedEventArgs e)
        {
            MoveToPoint("B");
        }

        private void PointC_Click(object sender, RoutedEventArgs e)
        {
            MoveToPoint("C");
        }

        private void PointD_Click(object sender, RoutedEventArgs e)
        {
            MoveToPoint("D");
        }

        private void MoveToPoint(string pointName)
        {
            switch (pointName)
            {
                case "A":
                    Console.WriteLine("Moving to point A");
                    break;
                case "B":
                    Console.WriteLine("Moving to point B");
                    break;
                case "C":
                    Console.WriteLine("Moving to point C");
                    break;
                case "D":
                    Console.WriteLine("Moving to point D");
                    break;
                default:
                    Console.WriteLine($"Moving to point {pointName}");
                    break;
            }
        }
    }
}
