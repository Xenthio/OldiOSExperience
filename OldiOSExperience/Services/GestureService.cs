using Microsoft.AspNetCore.Components.Web;

namespace OldiOSExperience.Services
{
    /// <summary>
    /// Service for handling touch and mouse gestures
    /// Inspired by iOS's UIGestureRecognizer system
    /// </summary>
    public class GestureService
    {
        /// <summary>
        /// Detects and handles swipe gestures
        /// </summary>
        public class SwipeGestureRecognizer
        {
            private bool isDragging = false;
            private double startX = 0;
            private double startY = 0;
            private double currentX = 0;
            private double currentY = 0;
            private bool isMouseDown = false;
            
            public double DragOffset => isDragging ? currentX - startX : 0;
            public bool IsDragging => isDragging;
            public SwipeDirection Direction { get; private set; } = SwipeDirection.None;
            
            // Events
            public event Action? OnSwipeStarted;
            public event Action<double>? OnSwipeChanged;
            public event Action<SwipeResult>? OnSwipeEnded;
            
            // Touch handlers
            public void HandleTouchStart(TouchEventArgs e)
            {
                if (e.Touches.Length > 0)
                {
                    isDragging = true;
                    startX = e.Touches[0].ClientX;
                    startY = e.Touches[0].ClientY;
                    currentX = startX;
                    currentY = startY;
                    OnSwipeStarted?.Invoke();
                }
            }
            
            public void HandleTouchMove(TouchEventArgs e)
            {
                if (!isDragging || e.Touches.Length == 0) return;
                
                currentX = e.Touches[0].ClientX;
                currentY = e.Touches[0].ClientY;
                
                var offsetX = currentX - startX;
                var offsetY = currentY - startY;
                
                UpdateSwipeDirection(offsetX, offsetY);
                OnSwipeChanged?.Invoke(offsetX);
            }
            
            public void HandleTouchEnd(TouchEventArgs e)
            {
                if (!isDragging) return;
                
                var result = GetSwipeResult();
                isDragging = false;
                OnSwipeEnded?.Invoke(result);
                
                Direction = SwipeDirection.None;
            }
            
            // Mouse handlers (for desktop testing)
            public void HandleMouseDown(MouseEventArgs e)
            {
                isMouseDown = true;
                isDragging = true;
                startX = e.ClientX;
                startY = e.ClientY;
                currentX = startX;
                currentY = startY;
                OnSwipeStarted?.Invoke();
            }
            
            public void HandleMouseMove(MouseEventArgs e)
            {
                if (!isMouseDown || !isDragging) return;
                
                currentX = e.ClientX;
                currentY = e.ClientY;
                
                var offsetX = currentX - startX;
                var offsetY = currentY - startY;
                
                UpdateSwipeDirection(offsetX, offsetY);
                OnSwipeChanged?.Invoke(offsetX);
            }
            
            public void HandleMouseUp(MouseEventArgs e)
            {
                if (!isMouseDown) return;
                
                isMouseDown = false;
                var result = GetSwipeResult();
                isDragging = false;
                OnSwipeEnded?.Invoke(result);
                
                Direction = SwipeDirection.None;
            }
            
            private void UpdateSwipeDirection(double offsetX, double offsetY)
            {
                // Determine primary direction based on which offset is larger
                if (Math.Abs(offsetX) > Math.Abs(offsetY))
                {
                    Direction = offsetX > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                }
                else
                {
                    Direction = offsetY > 0 ? SwipeDirection.Down : SwipeDirection.Up;
                }
            }
            
            private SwipeResult GetSwipeResult()
            {
                var offsetX = currentX - startX;
                var offsetY = currentY - startY;
                
                return new SwipeResult
                {
                    Direction = Direction,
                    OffsetX = offsetX,
                    OffsetY = offsetY,
                    Distance = Math.Sqrt(offsetX * offsetX + offsetY * offsetY)
                };
            }
        }
        
        /// <summary>
        /// Detects and handles tap gestures
        /// </summary>
        public class TapGestureRecognizer
        {
            private DateTime tapStartTime;
            private const int MaxTapDurationMs = 300;
            private const int MaxTapMovement = 10; // pixels
            
            private double startX = 0;
            private double startY = 0;
            
            public event Action? OnTap;
            public event Action? OnDoubleTap;
            
            private DateTime lastTapTime = DateTime.MinValue;
            private const int DoubleTapWindowMs = 300;
            
            public void HandleTouchStart(TouchEventArgs e)
            {
                if (e.Touches.Length > 0)
                {
                    tapStartTime = DateTime.Now;
                    startX = e.Touches[0].ClientX;
                    startY = e.Touches[0].ClientY;
                }
            }
            
            public void HandleTouchEnd(TouchEventArgs e)
            {
                var duration = (DateTime.Now - tapStartTime).TotalMilliseconds;
                var changedTouch = e.ChangedTouches.FirstOrDefault();
                
                if (changedTouch != null && duration <= MaxTapDurationMs)
                {
                    var moveDistance = Math.Sqrt(
                        Math.Pow(changedTouch.ClientX - startX, 2) +
                        Math.Pow(changedTouch.ClientY - startY, 2)
                    );
                    
                    if (moveDistance <= MaxTapMovement)
                    {
                        var timeSinceLastTap = (DateTime.Now - lastTapTime).TotalMilliseconds;
                        
                        if (timeSinceLastTap <= DoubleTapWindowMs)
                        {
                            OnDoubleTap?.Invoke();
                            lastTapTime = DateTime.MinValue; // Reset to prevent triple tap
                        }
                        else
                        {
                            OnTap?.Invoke();
                            lastTapTime = DateTime.Now;
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Swipe direction enum
    /// </summary>
    public enum SwipeDirection
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
    
    /// <summary>
    /// Result of a swipe gesture
    /// </summary>
    public class SwipeResult
    {
        public SwipeDirection Direction { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public double Distance { get; set; }
    }
}
