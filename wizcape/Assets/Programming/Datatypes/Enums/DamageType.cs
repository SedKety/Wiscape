using System;
using UnityEngine;

[Flags]
public enum DamageType
{
    physical = blunt | sharp,
    blunt = 1,
    sharp = 2,
    fire = 4,
    ice = 8,
    electric = 16,
    poison = 32,
    psychic = 64,
    holy = 128,
    unholy = 256,
    explosive = 512,
    water = 1024,
    magic = 2048,
}
