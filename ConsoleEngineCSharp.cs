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
	public int Width{
		get{return Console.BufferWidth;}
	}
	public int Height{
		get{return Console.BufferHeight;}
	}
	public Window(int width, int height, string title){
		Console.Title = title;
		Console.OutputEncoding = System.Text.Encoding.Unicode;
		if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)){
			Console.SetBufferSize(width, height);
		}
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
		old = DateTime.Now.TimeOfDay;
		gameloop();
	}

	public (int w, int h) getSize(){
		(int w, int h) size;
		size.w = Console.BufferWidth;
		size.h = Console.BufferHeight;
		return size;
	}

	private void gameloop(){
		while(running){
			var now = DateTime.Now.TimeOfDay;
			double calc = now.Subtract(old).TotalSeconds;
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
			if (x >= window.Width) break;
			for (int y = 0; y < width; y++){
				if (y >= window.Height) break;
				Console.ForegroundColor = foregroundColors[x, y];
				Console.BackgroundColor = backgroundColors[x, y];
				Console.SetCursorPosition(posx + x, posy + y);
				Console.Write(characters[x,y]);
			}
		}
		Console.ResetColor();
	}

	class Draw{
		public static Rectangle drawRectangle(Canvas canvas,Vector2Int position, int width, int height, char character, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, bool fill = true){
			return drawRectangle(canvas, new Rectangle(position, width, height), character, foregroundColor, backgroundColor, fill);
		}
		public static Rectangle drawRectangle(Canvas canvas, Rectangle rect, char character, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, bool fill = true){
			//check and resize the rect to the canvas to hinder it from throwing out of bounds
			Rectangle rectfordrawing = checkBounds(canvas, rect);

			//set the 'pixels' on the canvas
			for (int x = rectfordrawing.Origin.X; x < rectfordrawing.Origin.X + rectfordrawing.Width; x++){
				for (int y = rectfordrawing.Origin.Y; y < rectfordrawing.Origin.Y + rectfordrawing.Height; y++){
					if(fill){
						canvas.characters[x, y] = character;
						canvas.foregroundColors[x, y] = foregroundColor;
						canvas.backgroundColors[x, y] = backgroundColor;
					}else if(x == rect.Origin.X || x == rect.Origin.X + rect.Width || y == rect.Origin.Y || y == rect.Origin.Y + rect.Height){
						canvas.characters[x, y] = character;
						canvas.foregroundColors[x, y] = foregroundColor;
						canvas.backgroundColors[x, y] = backgroundColor;
					}
				}
			}
			return rect;
		}
		public static Rectangle drawCircle(Canvas canvas, Vector2Int position, int radius, char character, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, bool fill = true){
			Rectangle rect = new Rectangle(position, radius * 2, radius * 2);
			Rectangle fordrawing = checkBounds(canvas, rect);
			//TODO add draw functionality to circle
			
			for (float i = 0; i < 360; i += 0.1f){
				float angle = i;
				int x1 = (int)(radius * MathF.Cos(angle * MathF.PI / 180));
				int y1 = (int)(radius * MathF.Sin(angle * MathF.PI / 180));
				if(fordrawing.collidesWith(new Vector2Int(x1, y1))){
					canvas.backgroundColors[x1, y1] = backgroundColor;
					canvas.foregroundColors[x1, y1] = foregroundColor;
					canvas.characters[x1, y1] = character;
					if (fill){
						if(x1 <= position.X + radius){
							for (int x2 = x1; x1 <= position.X + radius; x2++){
								canvas.backgroundColors[x2, y1] = backgroundColor;
								canvas.foregroundColors[x2, y1] = foregroundColor;
								canvas.characters[x2, y1] = character;
							}
						}else{
							for (int x2 = x1; x1 <= position.X + radius; x2--){
								canvas.backgroundColors[x2, y1] = backgroundColor;
								canvas.foregroundColors[x2, y1] = foregroundColor;
								canvas.characters[x2, y1] = character;
							}
						}
					}
				}
			}
			return rect;
		}
		public static void drawLine(Canvas canvas, Vector2Int start, Vector2Int end, char character, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, bool fill = true){
			Vector2Int test = end - start;

			if(test.X < 0){
				Rectangle drawRect = new Rectangle(end, test.X, test.Y);
				for (int x = end.X; x < start.X; x++){
					int y = start.Y - test.Y  * (x - start.X) / test.X;
					if(drawRect.collidesWith(new Vector2Int(x, y))){
						canvas.foregroundColors[x,y] = foregroundColor;
						canvas.backgroundColors[x,y] = backgroundColor;
						canvas.characters[x,y] = character;
					}
				}
			}else{
				Rectangle drawRect = new Rectangle(start, test.X, test.Y);
				for (int x = start.X; x < end.X; x++){
					int y = start.Y - test.Y  * (x - start.X) / test.X;
					if(drawRect.collidesWith(new Vector2Int(x, y))){
						canvas.foregroundColors[x,y] = foregroundColor;
						canvas.backgroundColors[x,y] = backgroundColor;
						canvas.characters[x,y] = character;
					}
				}
			}
		}
		public static void drawText(Canvas canvas, Vector2Int position, string text, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, bool vertical = false){
			var textarray = text.ToCharArray();
			if(vertical){
				for (int y = position.Y; y < position.Y + textarray.Length; y++){
					if (y >= canvas.height || position.X >= canvas.width) break;
					if (position.X + text.Length < canvas.width){
						canvas.backgroundColors[position.X, y] = backgroundColor;
						canvas.foregroundColors[position.X, y] = foregroundColor;
						canvas.characters[position.X, y] = textarray[y - position.Y];
					}
					return;
				}
			}
			for (int x = position.X; x < position.X + textarray.Length; x++){
				if (x >= canvas.width || position.Y >= canvas.height) break;
				if (position.X + text.Length < canvas.width){
					canvas.backgroundColors[x, position.Y] = backgroundColor;
					canvas.foregroundColors[x, position.Y] = foregroundColor;
					canvas.characters[x, position.Y] = textarray[x - position.X];
				}
			}
		}
		public static void drawChar(Canvas canvas, Vector2Int position, char character, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black){
			if(new Rectangle(new Vector2Int(0, 0), canvas.width, canvas.height).collidesWith(position)){
				canvas.characters[position.X, position.Y] = character;
				canvas.foregroundColors[position.X, position.Y] = foregroundColor;
				canvas.backgroundColors[position.X, position.Y] = backgroundColor;
			}
		}
		private static Rectangle checkBounds(Canvas canvas, Rectangle rect){
			//check and resize the rect to the canvas to hinder it from throwing out of bounds
			rect.Width = rect.Origin.X + rect.Width < canvas.width ? rect.Width : canvas.width - rect.Origin.X;
			rect.Height = rect.Origin.Y + rect.Height < canvas.height ? rect.Height : canvas.height - rect.Origin.Y;
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

	public static Vector2Int operator +(Vector2Int vec1, Vector2Int vec2){
		return new Vector2Int(vec1.X + vec2.X, vec1.Y + vec2.Y);
	}
	public static Vector2Int operator -(Vector2Int vec1, Vector2Int vec2){
		return new Vector2Int(vec1.X - vec2.X, vec1.Y -vec2.Y);
	}
}

class Rectangle{
	private Vector2Int _origin;
	private int _width;
	private int _height;
	public int Width{
		get{return _width;}
		set{_width = value;}
	}
	public int Height{
		get{return _height;}
		set{_height = value;}
	}
	public Vector2Int Origin{
		get{return _origin;}
	}
	public Rectangle(Vector2Int origin, int width, int height){
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