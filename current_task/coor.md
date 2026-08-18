Base #1 (Upper Right) = Red

Center: 94.2, 0.0, -101.4

Corner	Position
Upper Left	73.5, 0.0, -109.6
Upper Right	109.9, 0.0, -110.8
Bottom Right	108, 0.0, -85
Bottom Left	75, 0.0, -85

Base #2 (Bottom Left) = Blue — mirrored

Center listed: -96.7, 0.0, 3.3  (Z does not sit in the rectangle; scripts use corner average instead: -91.6, 0.0, 97.6)

Corner	Position	Mirrors Base #1
Top Left	-108, 0.0, 85	BR
Top Right	-75, 0.0, 85	BL
Bottom Right	-73.5, 0.0, 109.6	UL
Bottom Left	-109.9, 0.0, 110.8	UR

Script mapping (MOBA/CoordinateFallbacks.cs):

- BlueFountain: back of Base #2 (-102.6, 0, 105.5)
- BlueCastle: Base #2 corner average (-91.6, 0, 97.6)
- BlueGate: front of Base #2 toward mid (-83.3, 0, 91.3)
- BlueTower: 35% of the way from Blue castle to mid (-59.1, 0, 62.8)
- MidLane1: midpoint of the two bases (1.3, 0, -1.9)
- RedTower: 35% of the way from Red castle to mid (61.7, 0, -66.6)
- RedGate: front of Base #1 toward mid (84.6, 0, -93.2)
- RedCastle: Base #1 center (94.2, 0, -101.4)
- RedFountain: back of Base #1 (102.1, 0, -106.1)
