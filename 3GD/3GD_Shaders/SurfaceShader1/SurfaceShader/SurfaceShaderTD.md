## Surface Shader TD. 

Objectif : Manipuler un surface shader standard. 

### TD1

#### Mise en place.
* Cr�ez une copie du shader MinimalPBRSurfaceShader.shader en MinimalPBRSurfaceShaderWithDecal.
* Cr�ez une copie du material CustomParquet lui affecter le nouveau shader.
* Drag n dropper le prefab CustomParquet le placer juste a cot� de l'autre.
* V�rifiez que c'est le bon materiel qui est branch� en modifiant le surface shader et en constatant le changement.

#### Ajout d'un canal UV
* Ajouter un canal custom nomm� par exemple CustomUV dans la struct Input.
* Lui affecter v.vertex.xz dans le surface shader modifier l'albedo en utilisant ce canal pour v�rifier que c'est correctement branch�.
* Ajouter une texture _Decal dans le shader, dans le materiel branch� une texture, utiliser le canal custom uv pour lire la texture.

#### Selection de la zone.
* Ecrire un test qui permet de n'afficher le Decal que lorsque la valeur de CustomUV est compris entre 0 et 1.
* Rendre conditionnel la lecture de la texture _Decal lorsqu'on est en dehors de ces conditions.

#### Utilisation de la matrice _WorldToLocal
* Ajouter un uniform de type matrice de nom _WorldToLocal
* Lire et comprendre le script AddWorldToLocalMatrix
* Utiliser la matrice _WorldToLocal pour calculer CustomUV. Constater ce qui se passe lorsque vous bougez le cube blanc.

#### Plus loin. (option)
* Ajouter d'autres canaux dans le decal.
* Faite un trou dans le parquet en utilisant le decal.
* Utiliser un shader provenant d'un cours pr�c�dent en utilisant le canal uv comme entr�e.
* Refaire la tarte a la creme du m�lange de deux mati�res en utilisant un masque.

#### TD2

Objectif : Manipuler le temps. Manipuler des shaders complexes.

#### Apparition disparition

* Ajouter un param�tre de shader _Visibility, l'utiliser pour afficher ou pas le decal. 
* le faire apparaitre comme un range en utilisant l'attribut idoine. https://docs.unity3d.com/Manual/SL-Properties.html
* (option) On peut utiliser d'autre mode d'apparition qu'un bete alpha. Par exemple utiliser un bruit, faire des bandes de plus en plus epaisse, faire une apparition de bas en haut ou droite gauche, etc... 
* Ajouter un usage de _UnityTime pour animer la visibilit� du decal.

#### Export du temps
* Ajouter un nouveau game objet vide, le mettre en haut de la hierarchie, lui ajouter un component ExportTimeOfTheDay.
* Pour tester utiliser les shader de clock pour faire apparaitre l'heure.
* (option) Transformer ce shader pour afficher le uniform float4 _UnityEngineTime.
Pour cela on change le numberToDisplay. On affiche les centi�mes de secondes, puis les secondes, puis les minutes. Cela fabrique un nombre a afficher qu'on transmet ensuite a la suite du shader.
* (option) Transformer ce shader en surface shader.

#### Value switcher et phylosophie.
* Ajouter un component _ValueSwitcher a cot� du renderer de decal. Lire le code du c# et comprendre l'encodage de l'animation.
* Ajouter le code pour decoder l'animation et la jouer en shader en remplacant _Visibility.

#### Draw procedural.
* A partir du MinimalGeometryShader faire une copie, cr�er un material. Ajouter un objet avec un component DrawProceduralRenderer. Tester.
* Utiliser Halton pour generer des points al�atoires.
* Faire de la neige.

