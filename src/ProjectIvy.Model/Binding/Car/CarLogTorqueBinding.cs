namespace ProjectIvy.Model.Binding.Car
{
    public class CarLogTorqueBinding
    {
        public long Time { get; set; }

        /// <summary>
        /// Throttle position
        /// </summary>
        public decimal? K11 { get; set; }

        /// <summary>
        /// Trip distance
        /// </summary>
        public decimal? Kff1204 { get; set; }

        /// <summary>
        /// Acceleration X
        /// </summary>
        public decimal? Kff1220 { get; set; }

        /// <summary>
        /// Acceleration Y
        /// </summary>
        public decimal? Kff1221 { get; set; }

        /// <summary>
        /// Acceleration Z
        /// </summary>
        public decimal? Kff1222 { get; set; }

        /// <summary>
        /// Acceleration total
        /// </summary>
        public decimal? Kff1223 { get; set; }

        /// <summary>
        /// Barometric pressure
        /// </summary>
        public decimal? K33 { get; set; }

        /// <summary>
        /// Engine load
        /// </summary>
        public decimal? K4 { get; set; }

        /// <summary>
        /// Ambient air temperature
        /// </summary>
        public decimal? K46 { get; set; }

        /// <summary>
        /// Fuel level
        /// </summary>
        public decimal? K2F { get; set; }

        /// <summary>
        /// Engine coolant temperature
        /// </summary>
        public decimal? K5 { get; set; }

        /// <summary>
        /// Intake manifold pressure
        /// </summary>
        public decimal? Kb { get; set; }

        /// <summary>
        /// Engine RPM
        /// </summary>
        public decimal? Kc { get; set; }

        /// <summary>
        /// Speed (OBD)
        /// </summary>
        public decimal? Kd { get; set; }

        /// <summary>
        /// Intake air temperature
        /// </summary>
        public decimal? Kf { get; set; }

        /// <summary>
        /// Torque log session
        /// </summary>
        public string Session { get; set; }
    }
}
