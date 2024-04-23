using DG.Tweening;
using Drawing;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

namespace DailyMeals
{
    public class FoodItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _foodItemImg;
        [SerializeField] private TMP_Text _foodPriceText;
        private Food _food;
        Customer selectedCustomer = null;
        Customer previousCustomer = null;

        public void SetUp(Food food)
        {
            _food = food;
            _foodItemImg.sprite = food.FoodImage;
            // _foodNameText.text = food.FoodName;
            _foodPriceText.text = $"${food.Price}";
            _button.onClick.AddListener(DisplayFoodDetail);
        }

        private void DisplayFoodDetail()
        {
            Debug.Log($"显示{_food.FoodName}详情...");
        }

        private void Update()
        {
            if (Input.touchCount == 1)
            {
                if (Camera.main == null) return;
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.3f, 0.7f);
                    if (Camera.main == null) return;
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                    Draw.Ray(ray, 10000);
                    SpawnManager.Singleton.IsDragging = true;

                    if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
                    {
                        SpawnManager.Singleton.StartPos = hitInfo.point;
                    }
                }

                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                    if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Customer")))
                    {
                        if (hitInfo.transform.TryGetComponent(out Customer customer) &&
                            !SpawnManager.Singleton.StartPos.Equals(default))
                        {
                            SpawnManager.Singleton.EndTransform = customer.transform;
                            selectedCustomer = customer;
                            if (previousCustomer != null && selectedCustomer != previousCustomer)
                            {
                                previousCustomer.UnSelected();
                            }

                            selectedCustomer.Selected();
                            previousCustomer = selectedCustomer;
                            using (Draw.ingame.WithDuration(0.1f))
                            {
                                using (Draw.ingame.WithLineWidth(2f))
                                {
                                    Draw.ingame.Circle(SpawnManager.Singleton.EndTransform.position, Vector3.up, 0.3f,
                                        Color.cyan);
                                }
                            }

                            if (GameManager.Singleton.FreeSeats.Count > 0)
                            {

                            }
                        }
                    }
                }

                if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 0.5f);

                    SpawnManager.Singleton.IsDragging = false;
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                    if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Customer")))
                    {
                        if (hitInfo.transform.TryGetComponent(out Customer customer))
                        {
                            selectedCustomer = customer;
                            if (previousCustomer != null && selectedCustomer != previousCustomer)
                            {
                                previousCustomer.UnSelected();
                            }

                            selectedCustomer.Selected();
                            previousCustomer = selectedCustomer;
                            if (GameManager.Singleton.FreeSeats.Count > 0&&!customer.IsWorking)
                            {
                                customer.OrderMeal(this._food);
                            }
                            else
                            {
                                Debug.Log($"{name}:没有空位了");
                            }
                        }
                    }
                    else
                    {
                        SpawnManager.Singleton.EndTransform = null;
                    }
                    if (selectedCustomer != null)
                    {
                        selectedCustomer.UnSelected();
                        previousCustomer = null;
                        selectedCustomer = null;
                    }
                }
            }
        }
    }

    // public void OnBeginDrag(PointerEventData eventData)
    // {
    //     if(Camera.main==null)return;
    //     Ray ray = Camera.main.ScreenPointToRay(eventData.position);
    //     Draw.Ray(ray,10000);
    //     SpawnManager.Singleton.IsDragging = true;
    //     
    //     if (Physics.Raycast(ray, out RaycastHit hitInfo,Mathf.Infinity,LayerMask.GetMask("Ground")))
    //     {
    //         
    //          SpawnManager.Singleton.StartPos= hitInfo.point;
    //     }
    // }
    //
    // public void OnDrag(PointerEventData eventData)
    // {
    //     if(Camera.current==null)return;
    //
    //     foreach (var hoveredObj in eventData.hovered)
    //     {
    //         if(hoveredObj.TryGetComponent(out Customer customer)&&!SpawnManager.Singleton.StartPos.Equals(default))
    //         {    
    //             SpawnManager.Singleton.EndTransform = customer.transform;
    //             selectedCustomer = customer;
    //             if(previousCustomer!=null&&selectedCustomer!=previousCustomer){previousCustomer.UnSelected();}
    //             selectedCustomer.Selected();
    //             previousCustomer = selectedCustomer;
    //             using (Draw.ingame.WithDuration(0.1f))
    //             {
    //
    //                 using (Draw.ingame.WithLineWidth(2f))
    //                 {
    //                     Draw.ingame.Circle(SpawnManager.Singleton.EndTransform.position, Vector3.up, 0.3f,Color.cyan);
    //                 }
    //             }
    //             if (GameManager.Singleton.FreeSeats.Count>0)
    //             {
    //                 
    //             }
    //         }
    //     }
    // }
    //
    // public void OnEndDrag(PointerEventData eventData)
    // {
    //     if (selectedCustomer != null)
    //     {
    //         selectedCustomer.UnSelected();
    //         previousCustomer = null;
    //         selectedCustomer = null;
    //     }
    //     
    //     SpawnManager.Singleton.IsDragging = false;
    //     foreach (var hoveredObj in eventData.hovered)
    //     {
    //         if(hoveredObj.TryGetComponent(out Customer customer))
    //         {
    //             if (GameManager.Singleton.FreeSeats.Count>0)
    //             {
    //                 customer.OrderMeal(this._food);
    //             }
    //             else
    //             {
    //                 Debug.Log($"{name}:没有空位了");
    //             }
    //             // break;
    //         }
    //     }
    // }
}

