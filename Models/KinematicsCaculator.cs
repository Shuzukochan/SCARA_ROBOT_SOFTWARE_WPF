using System;

namespace SCARA_ROBOT_SOFTWARE_WPF.Models
{
    public class KinematicsCaculator
    {
        public double L1 { get; set; }
        public double L2 { get; set; }

        public double Q1 { get; private set; }
        public double Q3 { get; private set; }
        public double D2 { get; private set; }
        public double D3 { get; private set; }

        public KinematicsCaculator(double l1 = 5, double l2 = 5)
        {
            L1 = l1;
            L2 = l2;
        }

        public void Inverse(double Px, double Py, double Pz, double Yaw)
        {
            D2 = Pz - L1 + L2;

            Q1 = Math.Atan2(Py, Px) * 180.0 / Math.PI; 

            if (Q1 < -135) Q1 = -135;
            if (Q1 > 135) Q1 = 135;

            double q1Rad = Q1 * Math.PI / 180.0;
            if (Math.Abs(Q1 - 90.0) < 1e-4)
                D3 = Py / Math.Sin(q1Rad);
            else
                D3 = Px / Math.Cos(q1Rad);

            Q3 = Yaw - Q1;
        }

        public (double x, double y, double z, double yaw) Forward(double q1, double q3, double d2, double d3)
        {
            double q1Rad = q1 * Math.PI / 180.0;
            double x = d3 * Math.Cos(q1Rad);
            double y = d3 * Math.Sin(q1Rad);
            double z = d2 + L1 - L2;
            double yaw = q1 + q3;
            return (x, y, z, yaw);
        }
    }
}
