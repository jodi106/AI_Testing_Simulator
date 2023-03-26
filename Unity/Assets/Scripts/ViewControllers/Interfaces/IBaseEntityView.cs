﻿using Assets.Enums;
using Entity;
using UnityEngine;
using System;


public interface IBaseEntityView : IBaseView
{
    public void onChangeCategory(AdversaryCategory cat);

    public void onChangeModel(EntityModel model);

    public void onChangeID(string id);
}