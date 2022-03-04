using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public UnitBase UnitBase { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }
    public int Mana { get; set; }

    public List<Move> Moves { get; set; }

    public Unit(UnitBase uBase, int level)
    {
        this.UnitBase = uBase;
        this.Level = level;
        HP = MaxHP;
        Mana = MaxMana;

        //Generate Moves
        Moves = new List<Move>();
        foreach(var move in uBase.LearnableMoves)
        {
            if(move.LevelToLearn <= level)
            {
                Moves.Add(new Move(move.MoveBase));
            }
        }
    }

    public int Attack
    {
        get { return UnitBase.Damage * this.Level; }
    }
    public int Defense
    {
        get { return UnitBase.Defense * this.Level; }
    }
    public int MaxHP
    {
        get { return UnitBase.MaxHP * this.Level; }
    }
    public int MaxMana
    {
        get { return UnitBase.MaxMana * this.Level; }
    }
    public int Speed
    {
        get { return UnitBase.Speed * this.Level; }
    }
}
