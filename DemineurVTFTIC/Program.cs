// Demineur

// declaration d'un nombre aleatoire
Random rand = new Random();

// declaration du champ de mine 
Tile[,] field = new Tile[10,10];

// remplir les champ de mine avec une fonction
FillBombs(field, 10);
DisplayFlied(field);

while (true)  // mettre les conditions de victoire
{
    Console.SetCursorPosition(0, 0);
    Console.Write(" Horizontal = ");
    if(int.TryParse(Console.ReadLine(), out int x))
    {
        Console.Write(" Vertical = ");
        if (int.TryParse(Console.ReadLine(), out int y))
        {
            CheckTile(field, x, y);  // va mettre la case en visile
            DisplayFlied(field);      // affiche la grille
            if(field[x,y].Value == 9)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("BOUUUUM !!!!");
                Console.ResetColor();
                break;
            }
        }
        else
        {
            Console.WriteLine("doit etre un nombre compris entre 0 et 9 !");
        }
    }
    else
    {
        Console.WriteLine("doit etre un nombre compris entre 0 et 9 !");
    }
}


void CheckTile(Tile[,] field, int x, int y)
{
    if (field[x, y].IsVisible) // si la case est deja decouverte sortir de la fonction evite une boucle infinie !!
    {
        return;
    }
    field[x,y].IsVisible = true;
    if(field[x,y].Value == 0)
    {
        // convoluer et checker toutes les case autour
        Convoluate(field, x, y, CheckTile);
    }

}

void DisplayFlied(Tile[,] field)
{
    for (int y = 0; y < field.GetLength(1); y++)
    {
        for (int x = 0; x < field.GetLength(0); x++)
        {
            Console.SetCursorPosition(x * 2 + 5, y + 2 );  // *2 permet d'ecarter l'affichage  // +5 , +2 permet de decaller le tableau dans la console
            if(field[x,y].Value == 9 && field[x,y].IsVisible)  // si la valeur = 9 et que la case doit etre affichée
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.Write(field[x, y].IsVisible ? field[x,y].Value : "♦");
            Console.ResetColor();   
         }
       
    }
}



void FillBombs(Tile [,] field, int nbBombs)
{
    for (int i = 0; i < nbBombs; i++)
    {
        int x, y;
        do
        {
            x = rand.Next(0, field.GetLength(0));
            y = rand.Next(0, field.GetLength(1));
        } while (field[x,y].Value == 9); // tant que la valeur est 9, je recrée des nouveau nombre aleatoire
        field[x, y].Value = 9;  // la valeur 9 est une bombe
        // augementer d'une unité la valeur des cases autours d'un bombe  // algorithme de convolution
        Aura(field, x, y);

    }
}

void Aura(Tile[,] field, int x, int y)
{
    Convoluate(field, x, y, (field , dx ,dy) =>
            {
                field[dx,dy].Value++;
            });
     
}

void Convoluate(Tile[,] field, int x, int y, Action< Tile[,],int, int > action )  // action est un delegue en paramettre
{

    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            // condition pour incrementer 
            if (!(                 // inversion des conditions 
                i == 0 && j == 0 // je suis sur la case    
                || x + j < 0    // si je sors a gauche
                || x + j > field.GetLength(0) - 1  // si je sors a droite 
                || y + i < 0   // si je sors en haut
                || y + i > field.GetLength(1) - 1 // si je sors en bas
                || field[x + j, y + i].Value == 9 // si il y a deja une bombe 
                )
             )
            {
                action(field, x + j, y + i);
            }
        }
    }
}

struct Tile
{
    public int Value { get; set; }
    public bool IsVisible { get; set; }
}