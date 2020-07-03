# Match3-test

Match3 is a tile matching game in which the player must create sequences of 3 or more tiles of the same type.

<p align="center">
  <img src="https://github.com/Wesley-Oliveira/match3-test/blob/master/match3Screenshot.png">
</p>

## Game concept

There are gems of different colors on a grid. Players have to move these gems up down left right to form a row or column of same colored 3 or more gems. When the player match gems of same kind, they gets destroyed and player get points for it. 
Everytime the player scores, the gems need to be moved down and freeing up place for new gems that will be automaticly generated on top.

**Rules**

- Game must be coded in C# using Unity 2018 or later
- After every change in the board the game must evaluate if there is possible moves and shuffle gems if needed.
- Every round must last 2 minutes and have a points goal that will bee increased after the conclusion of each round.
- Game must have sound and make use of the sprites and particles of this repository. 
- Be aware that the board and UI must works in differenct resolutions and aspect ratio (Portrait).
- Delivery: the project must be uploaded into a github repository

## Unity Version

The project was developed using Unity 2019.3.11f1

## Getting Started

- Clone this repository;

- Use Unity to open the local clone of this repository;

- In the Unity editor, select the "Menu.unity" scene.

Note: The game runs on a reference aspect ratio of 9:16.

## Controls

- To navigate and interact through the game, use your left mouse click;

**To move pieces, you can use two ways**

- The first is selecting the piece you want to move and then selecting the destination;

- The second is to select the piece you want to move, hold, drag to the destination and drop it.

## Credits

**Audio & Sprites**

All sounds, sprites were taken from [PlayKids](https://github.com/PlayKids/match3-test) repository.
