// Demineur

// declaration d'un nombre aleatoire
Random rand = new Random();
bool IsIsNbLigneOk = false;
bool IsIsNbColOk = false;
bool IsIsNbBombOk = false;
int nbLigne = 0;
int nbCol = 0;
int nbBomb = 0;
do
{
    Console.Write("Encodez le nombre le lignes : ");
    IsIsNbLigneOk = int.TryParse(Console.ReadLine(), out nbLigne);
} while (!IsIsNbLigneOk);

do
{
    Console.Write("Encodez le nombre le colones : ");
    IsIsNbColOk = int.TryParse(Console.ReadLine(), out nbCol);
} while (!IsIsNbColOk);

do
{
    Console.Write("Encodez le nombre le bombes : ");
    IsIsNbBombOk = int.TryParse(Console.ReadLine(), out nbBomb);
} while (!IsIsNbBombOk);

Console.Clear();

// declaration du champ de mine 
Tile[,] field = new Tile[nbLigne, nbCol];

// postion initial du curseur
(int x, int y) = (0, 0); 

// remplir les champ de mine avec une fonction
FillBombs(field, nbBomb);

// affichage
DisplayFlied(field);


do
{
    (x, y) = Move(field, x, y); // renvois la position du curseur 
    CheckTile(field, x, y);  // va mettre la case en visile
    DisplayFlied(field);      // affiche la grille

} while (WinCondition(field,x,y));  // mettre les conditions de victoire


Console.ReadKey();  


bool WinCondition(Tile[,] field, int x ,int y)
{
    if (field[x, y].Value == 9)
    {
        Console.SetCursorPosition(0,0);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Vous avez perdu");
        Console.ResetColor();
        return false;
    }
    // si tout les il ne reste plus que des bombes vous avez gagné
    if ( CountRemaining(field) == 10)
    {
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("C'est gagné");
        Console.ResetColor();
        return false;
    }
     return true;
}

int CountRemaining(Tile[,] field)
{
    int sum = 0;
    for (int i = 0; i < field.GetLength(0); i++)
    {
        for (int j = 0; j < field.GetLength(1); j++)
        {
            if (!field[i, j].IsVisible)
            {
                sum++;
            }
        }
    }
    return sum;
}



(int x, int y) Move(Tile[,]field,int x, int y)
{
    ConsoleKey? key = null;
    while(key != ConsoleKey.Spacebar)
    {
        Console.SetCursorPosition(x * 2 + 5, y + 2);
        key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.UpArrow:
                if( y > 0 )
                    y--;
                break;

            case ConsoleKey.DownArrow:
                if( y < field.GetLength(1)-1 )
                    y++;
                break;

            case ConsoleKey.LeftArrow:
                if( x > 0 )
                    x--;
                break;

            case ConsoleKey.RightArrow:
                if( x < field.GetLength(0)-1)
                    x++;
                break;
            default:
                break;
        }
    }   
    return (x,y);
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