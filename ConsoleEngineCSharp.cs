namespace ConsoleEngineCSharp;
public class Window
{
	public Action<double> Updating; //double is seconds since last update
	public Action<double> Rendering; //double is seconds since last render
	public Action Loading;
	public Action Closing;

	private TimeSpan old;
	private bool running;
	public bool IsMultithreaded;
	private int width;
	private int height;
	public int Width{
		get{return width;}
	}
	public int Height{
		get{return height;}
	}
	public Window(int width, int height, string title){
		Console.Title = title;
		Console.BufferHeight = height;
		Console.BufferWidth = width;
		Console.OutputEncoding = System.Text.Encoding.Unicode;
		old = DateTime.Now.TimeOfDay;
		running = false;
		IsMultithreaded = false;
		Updating += doupdate;
		Rendering += dorender;
		Loading += doload;
		Closing += doclose;
	}

	public void Run(){
		Loading.Invoke();
		gameloop();
	}

	private void gameloop(){
		while(running){
			double calc = 0;
			var now = DateTime.Now.TimeOfDay;
			if(old != null){
				calc = now.Subtract(old).TotalSeconds; 
			}
			old = now;
			Updating.Invoke(calc);
			Rendering.Invoke(calc);
		}
		Closing.Invoke();
	}

	private void doupdate(double arg1){

	}
	private void dorender(double arg1){
		
	}

	private void doclose(){

	}
	private void doload(){

	}

	public void CloseWindow(){
		running = false;
	}
	public bool CheckKeyPressed(ConsoleKey key){
		if(Console.KeyAvailable){
			return Console.ReadKey().Key == key;
		}
		return false;
	}
}

class Canvas{
	private char[,] characters;
	private ConsoleColor[,] foregroundColors;
	private ConsoleColor[,] backgroundColors;
	private int width;
	private int height;
	public Canvas(int width, int height){
		this.width = width;
		this.height = height;
		characters = new char[width, height];
		foregroundColors = new ConsoleColor[width, height];
		backgroundColors = new ConsoleColor[width, height];
		for (int x = 0; x < width; x++){
			for (int y = 0; y < height; y++){
				backgroundColors[x,y] = ConsoleColor.Black;
				backgroundColors[x,y] = ConsoleColor.White;
				characters[x,y] = '#';
			}
		}
	}

	public void DrawToWindow(Window window, int posx = 0, int posy = 0){
		for (int x = 0; x < width; x++){
			for (int y = 0; y < height; y++){
				Console.ForegroundColor = foregroundColors[x, y];
				Console.BackgroundColor = backgroundColors[x, y];
				Console.SetCursorPosition(posx + x, posy + y);
				Console.Write(characters[x,y]);
			}
		}
		Console.ResetColor();
	}

	class Draw{
		public Rectangle drawRectangle(Canvas canvas,Vector2Int position, uint width, uint height, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, bool fill = true){
			return drawRectangle(canvas, new Rectangle(position, width, height), foregroundColor, backgroundColor, fill);
			
		}
		public Rectangle drawRectangle(Canvas canvas, Rectangle rect, ConsoleColor foregroundColor, ConsoleColor backgroundColor , bool fill = true){
			//TODO add the draw function
			for (int x = rect.Origin.X; x < rect.Origin.X + rect.Width; x++){
				if(fill){
					
				}else{

				}
			}
			return rect;
		}
	}
}

class Vector2Int{
	private int _X;
	private int _Y;

	public int X{
		get{return _X;}
		set{_X = value;}
	}
	public int Y{
		get{return _Y;}
		set{_Y = value;}
	}
	public float Length{
		get{
			var ulength = (_X*_X) + (_Y*_Y);
			float dlength = ulength;
			dlength = MathF.Sqrt(dlength);
			return dlength;
		}
	}
	public Vector2Int(int x, int y){
		_X = x;
		_Y = y;
	}
}

class Rectangle{
	private Vector2Int _origin;
	private uint _width;
	private uint _height;
	public uint Width{
		get{return _width;}
		set{_width = value;}
	}
	public uint Height{
		get{return _height;}
		set{_height = value;}
	}
	public Vector2Int Origin{
		get{return _origin;}
	}
	public Rectangle(Vector2Int origin, uint width, uint height){
		_origin = origin;
		_width = width;
		_height = height;
	}

	public bool collidesWith(Rectangle collider){
		bool xT = _origin.X >= collider._origin.X && _origin.X <= collider._origin.X + collider._width;
		bool yT = _origin.Y >= collider._origin.Y && _origin.Y <= collider._origin.Y + collider._height;
		bool xB = _origin.X + _width >= collider._origin.X && _origin.X + _width <= collider._origin.X + collider._width;
		bool yB = _origin.Y + _height >= collider._origin.Y && _origin.Y + _height <= collider._origin.Y + collider._height;
		return (xT || xB) && (yT || yB);
	}

	public bool collidesWith(Vector2Int point){
		return (point.X <= _origin.X + _width && point.X >= _origin.X && point.Y <= _origin.Y + _height && point.Y >= _origin.Y);
	}
	public void Move(int x, int y){
		Origin.X += x;
		Origin.Y += y;
	}
}