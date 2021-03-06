﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AbilityAction
{
   public Ability ability;
   public Tile tile;
    public List<Tile> tiles;

    public AbilityAction(Ability ability,Tile tile,List<Tile> tiles)
    {
        this.ability = ability;
        this.tile = tile;
        this.tiles = tiles;
    }
    public void SetTiles(List<Tile> tiles)
    {
        this.tiles = tiles;
    }
}

public class EffectAction
{
    public A_Effect effect;
    public Tile tile;
    public List<Tile> tiles;

    public EffectAction(A_Effect effect, Tile tile, List<Tile> tiles)
    {
        this.effect = effect;
        this.tile = tile;
        this.tiles = tiles;
    }

    public void SetTiles(List<Tile> tiles)
    {
        this.tiles = tiles;
    }
}
public class Indicator : MonoBehaviour {

    public Color fireColor;
    public Color thunderColor;
    public Color lightColor;
    public Color healColor;
    public Color normalColor;
    public Color poisonColor;

    public List<AbilityAction> activeAbilitys = new List<AbilityAction>();
    public List<EffectAction> activeEffects = new List<EffectAction>();

    //For Indicator in HumanPlayer on HoverAbility 
    public AbilityAction abilityNoCast;
    public List<EffectAction> NoCastEffects = new List<EffectAction>();
    bool noCast = false;
    bool noCastSet = false;


    void Start()
    {
        EventManager.OnMoveAction += Refresh;
        EventManager.OnAbilityAction += Refresh;
    }

    public void DrawAbility(Ability ability, Tile tile)
    {
        activeAbilitys.Add(new AbilityAction(ability, tile, ability.DrawIndicator(tile)));
    }



    public void Refresh()
    {
        for (int i = 0; i < activeAbilitys.Count; i++)
        {
            RemoveAbility(activeAbilitys[i]);
        }
        for (int i = 0; i < activeEffects.Count; i++)
        {
            RemoveEffect(activeEffects[i]);
        }

        for (int i = 0; i < activeAbilitys.Count; i++)
        {
            activeAbilitys[i].SetTiles(activeAbilitys[i].ability.DrawIndicator(activeAbilitys[i].tile));
        }
        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].SetTiles(activeEffects[i].effect.DrawIndicator(activeEffects[i].tile));
        }

    }

    public void DrawEffect(A_Effect effect, Tile tile)
    {
        if (noCast)
        {
            NoCastEffects.Add(new EffectAction(effect, tile, effect.DrawIndicator(tile)));
        }
        else
        {
            activeEffects.Add(new EffectAction(effect, tile, effect.DrawIndicator(tile)));
        }
    }


    public void DrawNotCastedAbility(Ability ability, Tile tile)
    {
        Refresh();
        noCast = true;
        abilityNoCast = new AbilityAction(ability, tile, ability.DrawIndicator(tile));
        noCast = false;
        noCastSet = true;
    }

    public void RemoveNoCast()
    {
        if (noCastSet)
        {
            RemoveAbility(abilityNoCast);
            for (int i = 0; i < NoCastEffects.Count; i++)
            {
                RemoveEffect(NoCastEffects[i]);
            }
            NoCastEffects = new List<EffectAction>();
            noCastSet = false;
            Refresh();
        }

    }
   

    public void RemoveAbility(Ability ability, Tile tile)
    {
        int index = activeAbilitys.FindIndex(x => (x.ability == ability && x.tile == tile));
        if (index != -1)
        {
            AbilityAction aa = activeAbilitys[index];
            RemoveAbility(aa);
            activeAbilitys.RemoveAt(index);
            Refresh();
        }
    }

    private void RemoveAbility(AbilityAction aa)
    {
        for (int i = 0; i < aa.tiles.Count; i++)
        {
            aa.tiles[i].tilehelper.Undo();
        }
    }


    public void RemoveEffect(A_Effect effect, Tile tile)
    {
        int index = activeEffects.FindIndex(x => (x.effect == effect && x.tile == tile));
        if (index != -1)
        {
            EffectAction ea = activeEffects[index];
            RemoveEffect(ea);
            activeEffects.RemoveAt(index);
            Refresh();
        }
    }
    private void RemoveEffect(EffectAction ea)
    {
        for (int i = 0; i < ea.tiles.Count; i++)
        {
            ea.tiles[i].tilehelper.Undo();
        }
    }

    public void DrawDamage(Tile tile, GameHelper.AbilityType type,int damage)
    {
        switch (type)
        {
            case GameHelper.AbilityType.Fire:
            case GameHelper.AbilityType.Thunder:
            case GameHelper.AbilityType.Light:
            case GameHelper.AbilityType.Normal:
            case GameHelper.AbilityType.Poison:
                if (noCast)
                {
                    tile.tilehelper.AbilityNoCast(damage, GetColor(type));
                }
                else
                {
                    tile.tilehelper.AbilityDefault(damage, GetColor(type));
                }
               
                break;
            case GameHelper.AbilityType.Heal:
                if (noCast)
                {
                    tile.tilehelper.AbilityNoCast(-damage, GetColor(type));
                }
                else
                {
                    tile.tilehelper.AbilityDefault(-damage, GetColor(type));
                }
                break;

        }

    }

    public void DrawIndicator(Tile tile, GameHelper.AbilityType type)
    {
        tile.tilehelper.AbilityTraverse(GetColor(type));
    }


    public Color GetColor(GameHelper.AbilityType type)
    {
        switch (type)
        {
            case GameHelper.AbilityType.Fire:
                return fireColor;
            case GameHelper.AbilityType.Thunder:
                return thunderColor;
            case GameHelper.AbilityType.Light:
                return lightColor;
            case GameHelper.AbilityType.Heal:
                return healColor;
            case GameHelper.AbilityType.Normal:
                return normalColor;
            case GameHelper.AbilityType.Poison:
                return poisonColor;
        }
        return new Color();
    }


    public void RemoveDamage(Tile tile)
    {
        tile.tilehelper.Undo();
        tile.tilehelper.HideText();
    }

    public void DrawEffect(Tile tile,A_Effect effect)
    {
        effect.DrawIndicator(tile);
    }
    public void RemoveEffect(Tile tile, GameHelper.EffectType effect)
    {
        switch (effect)
        {
            case GameHelper.EffectType.Burning:
                DrawDamage(tile, GameHelper.AbilityType.Fire,1);
                break;
            case GameHelper.EffectType.Thunder:
                break;
        }
    }

}
