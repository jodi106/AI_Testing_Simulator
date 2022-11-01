using Assets.Enums;
using Models;
using System.Collections.Generic;

namespace Assets.Repos
{
    public class PedestrianModelRepository
    {
        private readonly Dictionary<PedestrianType, List<Model>> _pedestrianModels;
        public PedestrianModelRepository()
        {
            _pedestrianModels = new Dictionary<PedestrianType, List<Model>>();

            _pedestrianModels.Add(PedestrianType.Woman, new List<Model>
            {
                new Model(1,"BP_Walker_Female1_v1"),
                new Model(2,"BP_Walker_Female1_v2"),
                new Model(3,"BP_Walker_Female1_v3"),
                new Model(4,"BP_Walker_Female1_v4"),
                new Model(5,"BP_Walker_Female1_v5"),
                new Model(6,"BP_Walker_FemaleAfro"),
                new Model(7,"BP_Walker_FemaleAfro02"),
                new Model(8,"BP_Walker_FemaleAfro02B"),
                new Model(9,"BP_Walker_FemaleAfro02C"),
                new Model(10,"BP_Walker_FemaleAfro02D"),
                new Model(11,"BP_Walker_FemaleAfro02_Cop"),
                new Model(12,"BP_Walker_FemaleAfro03_v1"),
                new Model(13,"BP_Walker_FemaleAfro03_v2"),
                new Model(14,"BP_Walker_FemaleAfro03_v3"),
                new Model(15,"BP_Walker_FemaleAfro_v2"),
                new Model(16,"BP_Walker_FemaleAsia_v1"),
                new Model(17,"BP_Walker_FemaleAsia_v2"),
                new Model(18,"BP_Walker_FemaleEuro"),
                new Model(19,"BP_Walker_FemaleEuro_v2"),
                new Model(20,"BP_Walker_Female_Euro_Owv_v1"),
                new Model(21,"BP_Walker_Female_Euro_Owv_v2"),
                new Model(22,"BP_Walker_Female_Euro_Owv_v3"),
            });


            _pedestrianModels.Add(PedestrianType.Man, new List<Model>
            {
                new Model(23,"BP_Walker_Male1_v1"),
                new Model(24,"BP_Walker_Male1_v2"),
                new Model(25,"BP_Walker_Male1_v3"),
                new Model(26,"BP_Walker_MaleAfro_v1"),
                new Model(27,"BP_Walker_MaleAfro_v2"),
                new Model(28,"BP_Walker_MaleAmer_Cop"),
                new Model(29,"BP_Walker_MaleAmer_v1"),
                new Model(30,"BP_Walker_MaleAmer_v2"),
                new Model(31,"BP_Walker_MaleAmer_v3"),
                new Model(32,"BP_Walker_MaleAsia_v1"),
                new Model(33,"BP_Walker_MaleAsia_v2"),
                new Model(34,"BP_Walker_MaleEuro,"),
                new Model(35,"BP_Walker_MaleEuro_v2"),
                new Model(36,"BP_Walker_Male_Afro02_S_Ov"),
                new Model(37,"BP_Walker_Male_Asia02_v1"),
                new Model(38,"BP_Walker_Male_Asia02_v2,"),
                new Model(39,"BP_Walker_Male_Asia02_v3"),
                new Model(40,"BP_Walker_Male_EuroW_Owv"),
                new Model(41,"BP_Walker_Male_EuroW_Owv_v2"),
            });

            _pedestrianModels.Add(PedestrianType.Girl, new List<Model>
            {
                new Model(42, "BP_WalkerEuGirl02_v1"),
                new Model(43, "BP_WalkerGirl1_v1"),
                new Model(44, "BP_WalkerGirl1_v2"),
                new Model(45, "BP_WalkerGirl1_v3"),
            });

            _pedestrianModels.Add(PedestrianType.Boy, new List<Model>
            {
                new Model(34, "BP_WalkerEuBoy02_v1,"),
                new Model(35, "BP_WalkerKid1_v1"),
                new Model(36, "BP_WalkerKid1_v2"),
                new Model(37, "BP_WalkerKid1_v3"),
            });

        }

        public List<Model> GetModelsBasedOnType(PedestrianType type)
        {
            return _pedestrianModels[type];
        }


    }
}
