namespace Action
{
    public class UterusDependInfo : DependInfo
    {
        public UterusDependInfo() : base()
        {

        }

        public override int actorLimit
        {
            get
            {
                return 2;
            }
        }

        public string motherIndex
        {
            get
            {
                return "0";
            }
        }

        public string childIndex
        {
            get
            {
                return "1";
            }
        }
    }
}
