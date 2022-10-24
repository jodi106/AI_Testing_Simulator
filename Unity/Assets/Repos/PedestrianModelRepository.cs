using Assets.Enums;
using Entities;
using System.Collections.Generic;

namespace Assets.Repos
{
    public class PedestrianModelRepository
    {
        private readonly Dictionary<PedestrianType, List<EntityModel>> _pedestrianModels;
        public PedestrianModelRepository()
        {
            _pedestrianModels = new Dictionary<PedestrianType, List<EntityModel>>();

            _pedestrianModels.Add(PedestrianType.Woman, new List<EntityModel>
            {
                new EntityModel(1,"BP_Walker_Female1_v1"),
                new EntityModel(2,"BP_Walker_Female1_v2"),
                new EntityModel(3,"BP_Walker_Female1_v3"),
                new EntityModel(4,"BP_Walker_Female1_v4"),
                new EntityModel(5,"BP_Walker_Female1_v5"),
                new EntityModel(6,"BP_Walker_FemaleAfro"),
                new EntityModel(7,"BP_Walker_FemaleAfro02"),
                new EntityModel(8,"BP_Walker_FemaleAfro02B"),
                new EntityModel(9,"BP_Walker_FemaleAfro02C"),
                new EntityModel(10,"BP_Walker_FemaleAfro02D"),
                new EntityModel(11,"BP_Walker_FemaleAfro02_Cop"),
                new EntityModel(12,"BP_Walker_FemaleAfro03_v1"),
                new EntityModel(13,"BP_Walker_FemaleAfro03_v2"),
                new EntityModel(14,"BP_Walker_FemaleAfro03_v3"),
                new EntityModel(15,"BP_Walker_FemaleAfro_v2"),
                new EntityModel(16,"BP_Walker_FemaleAsia_v1"),
                new EntityModel(17,"BP_Walker_FemaleAsia_v2"),
                new EntityModel(18,"BP_Walker_FemaleEuro"),
                new EntityModel(19,"BP_Walker_FemaleEuro_v2"),
                new EntityModel(20,"BP_Walker_Female_Euro_Owv_v1"),
                new EntityModel(21,"BP_Walker_Female_Euro_Owv_v2"),
                new EntityModel(22,"BP_Walker_Female_Euro_Owv_v3"),
            });


            _pedestrianModels.Add(PedestrianType.Man, new List<EntityModel>
            {
                new EntityModel(23,"BP_Walker_Male1_v1"),
                new EntityModel(24,"BP_Walker_Male1_v2"),
                new EntityModel(25,"BP_Walker_Male1_v3"),
                new EntityModel(26,"BP_Walker_MaleAfro_v1"),
                new EntityModel(27,"BP_Walker_MaleAfro_v2"),
                new EntityModel(28,"BP_Walker_MaleAmer_Cop"),
                new EntityModel(29,"BP_Walker_MaleAmer_v1"),
                new EntityModel(30,"BP_Walker_MaleAmer_v2"),
                new EntityModel(31,"BP_Walker_MaleAmer_v3"),
                new EntityModel(32,"BP_Walker_MaleAsia_v1"),
                new EntityModel(33,"BP_Walker_MaleAsia_v2"),
                new EntityModel(34,"BP_Walker_MaleEuro,"),
                new EntityModel(35,"BP_Walker_MaleEuro_v2"),
                new EntityModel(36,"BP_Walker_Male_Afro02_S_Ov"),
                new EntityModel(37,"BP_Walker_Male_Asia02_v1"),
                new EntityModel(38,"BP_Walker_Male_Asia02_v2,"),
                new EntityModel(39,"BP_Walker_Male_Asia02_v3"),
                new EntityModel(40,"BP_Walker_Male_EuroW_Owv"),
                new EntityModel(41,"BP_Walker_Male_EuroW_Owv_v2"),
            });

            _pedestrianModels.Add(PedestrianType.Girl, new List<EntityModel>
            {
                new EntityModel(42, "BP_WalkerEuGirl02_v1"),
                new EntityModel(43, "BP_WalkerGirl1_v1"),
                new EntityModel(44, "BP_WalkerGirl1_v2"),
                new EntityModel(45, "BP_WalkerGirl1_v3"),
            });

            _pedestrianModels.Add(PedestrianType.Boy, new List<EntityModel>
            {
                new EntityModel(34, "BP_WalkerEuBoy02_v1,"),
                new EntityModel(35, "BP_WalkerKid1_v1"),
                new EntityModel(36, "BP_WalkerKid1_v2"),
                new EntityModel(37, "BP_WalkerKid1_v3"),
            });

        }

        public IEnumerable<EntityModel> GetModelsBasedOnType(PedestrianType type)
        {
            return _pedestrianModels[type];
        }


    }
}
