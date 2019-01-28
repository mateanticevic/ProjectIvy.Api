namespace ProjectIvy.Model.Binding.Car
{
    public class CarLogTorqueBinding
    {
        public long Time { get; set; }

        /// <summary>
        /// Throttle position
        /// </summary>
        public double? K11 { get; set; }

        /// <summary>
        /// Barometric pressure
        /// </summary>
        public double? K33 { get; set; }

        /// <summary>
        /// Engine load
        /// </summary>
        public double? K4 { get; set; }

        /// <summary>
        /// Ambient air temperature
        /// </summary>
        public double? K46 { get; set; }

        /// <summary>
        /// Fuel level
        /// </summary>
        public double? K2F { get; set; }

        /// <summary>
        /// Engine coolant temperature
        /// </summary>
        public double? K5 { get; set; }

        /// <summary>
        /// Engine RPM
        /// </summary>
        public double? Kc { get; set; }

        /// <summary>
        /// Speed (OBD)
        /// </summary>
        public double? Kd { get; set; }

        /// <summary>
        /// Intake air temperature
        /// </summary>
        public double? Kf { get; set; }
    }
}
