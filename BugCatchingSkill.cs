using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;

namespace BugCatching
{
    public class BugCatchingSkill : SpaceCore.Skills.Skill
    {
        public class GenericProfession : SpaceCore.Skills.Skill.Profession
        {
            public GenericProfession(BugCatchingSkill skill, string itsId)
                : base( skill , itsId)
            {

            }

            internal string Name { get; set; }
            internal string Description { get; set; }

            public override string GetName()
            {
                return Name;
            }
            public override string GetDescription()
            {
                return Description;
            }
            
        }
        public BugCatchingSkill()
            : base("ded.bugCatching")
        {
            Icon = BugCatchingMod.instance.Helper.Content.Load<Texture2D>("Assets/skillIcon.png");
            SkillsPageIcon = BugCatchingMod.instance.Helper.Content.Load<Texture2D>("Assets/skillIcon.png");

            ExperienceCurve = new int[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 }; ;

            ExperienceBarColor = Microsoft.Xna.Framework.Color.DarkOrchid;

        }
        public override string GetName()
        {
            return "Bug Catching";
        }
        public override List<string> GetExtraLevelUpInfo(int level)
        {
            List<string> list = new List<string>();
            list.Add("better at the catch");
            return list;
        }
        public override string GetSkillPageHoverText(int level)
        {
            return "+" + (3 * level) + "% catching bonus";
        }
    }
}
