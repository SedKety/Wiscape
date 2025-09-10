using System;
using UnityEngine;

[Flags]
public enum DamageType
{
    physical = blunt | sharp,
    blunt = 0,
    sharp = 1,
    fire = 2,
    ice = 4,
    electric = 8,
    poison = 16,
    psychic = 32,
    holy = 64,
    unholy = 128,
    explosive = 256,
    water = 512,
}
