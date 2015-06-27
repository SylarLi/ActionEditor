public class TemplateConfig
{
    public PersonTemplates personTemplates = new PersonTemplates();

    public ITemplates GetTemplates(string name)
    {
        ITemplates templates = default(ITemplates);
        switch (name)
        {
            case "person":
                {
                    templates = personTemplates;
                    break;
                }
            default:
                {
                    throw new System.Exception("Template parse error : " + "can not find chart's template of " + name);
                }
        }
        return templates;
    }
}
