namespace CS2_Poor_MapPropAds.Models
{
    public class PropModel
    {
        public int Id { get; set; }
        public int ModelIndex { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public float angleX { get; set; }
        public float angleY { get; set; }
        public float angleZ { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public bool forceOnVip { get; set; }
        public bool isOnGround { get; set; }
    }
}