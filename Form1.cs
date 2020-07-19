using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace storefront
{
    public partial class Form1 : Form
    {
        //DollarAmount balance = new DollarAmount();
        DollarAmount orderTotal = new DollarAmount();
        Product[] products = //finite; array is useful
            {
                new Product(0, "placeholder 1", 10d),
                new Product(1, "placeholder 2", 20d),
                new Product(2, "placeholder 3", 30d)
            }; 
        List<ProductOrder> shoppingCart = new List<ProductOrder>(); //must be easy to change; thus, a list

        //use panels to make the different pages

        public Form1()
        {
            InitializeComponent();
            //balanceLabel.Text = balance.ToString();

            foreach(Product p in products)
            {
                listBox1.Items.Add(p);
            }

            listBox1.SelectedIndex = listBox1.TopIndex;

            resizeColumns(listView1);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Product currentProduct = (Product) listBox1.SelectedItem;
            int amtToAdd = (int) numSelect.Value;
            shoppingCart.Add(new ProductOrder(currentProduct, amtToAdd));

            double subtotal = currentProduct.Price.Amt * amtToAdd;

            ListViewItem order = new ListViewItem(currentProduct.Name); //this order is peculiar but necessary for the order to display properly
            order.SubItems.Add("" + amtToAdd);
            order.SubItems.Add(new DollarAmount(subtotal).ToString());
            order.SubItems.Add("" + currentProduct.Code);
            
            listView1.Items.Add(order);

            orderTotal.Amt += subtotal;
            textBox1.Text = orderTotal.ToString();

            resizeColumns(listView1);
        }

        private void resizeColumns(ListView listView1)
        {
            foreach (ColumnHeader column in listView1.Columns)
            {
                column.Width = -2;
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            for(int index = listView1.SelectedIndices.Count - 1; index >= 0; index--) //iterating backwards because iterating forwards can cause later indices to change when earlier items are removed
            {
                int i = listView1.SelectedIndices[index];

                orderTotal.Amt -= shoppingCart.ElementAt(i).OrderProduct.Price.Amt * shoppingCart.ElementAt(i).OrderAmt;
                textBox1.Text = orderTotal.ToString();

                shoppingCart.RemoveAt(i);
                listView1.Items.RemoveAt(i);
            }
        }

        private void checkoutButton_Click(object sender, EventArgs e)
        {
            checkOutput.Text = "Product Order: \n\n";
            foreach (ProductOrder p in shoppingCart)
            {
                checkOutput.Text += p.ToString() + "\n";
            }

            checkOutput.Text += $"\nTotal Cost: {textBox1.Text}";
        }
    }

    class ProductOrder //wraps a product and a # of that product to order
    {
        public Product OrderProduct { get; private set; }
        public int OrderAmt { get; private set; }

        public ProductOrder(Product prod, int amt)
        {
            OrderProduct = prod;
            OrderAmt = amt;
        }

        public override string ToString()
        {
            DollarAmount subtotal = new DollarAmount(OrderProduct.Price.Amt * OrderAmt);
            return $"{OrderAmt} of Product {OrderProduct.Code} ({OrderProduct.Name}) for {subtotal}";
        }
    }

    class Product
    {
        public int Code { get; private set; }
        public string Name { get; private set; }
        public DollarAmount Price { get; private set; }

        public Product(int code, string name, double price)
        {
            Code = code;
            Name = name;
            Price = new DollarAmount(price);
        }

        public override string ToString()
        {
            return $"{Name}: {Price.ToString()}";
        }

        /*public string toListBox()
        {
            return $"{Name}: {Price.ToString()}";
        } */
        //product code, name, description, price
    }

    class DollarAmount
    {
        private double _amt;
        public double Amt
        {
            get
            {
                return _amt;
            }
            set     //formats the input to two decimal places via truncating additional decimals
            {       //important since .toString("C") rounds values instead, which is not a desired behavior
                int temp = (int)(value * 100);
                _amt = (double)temp / 100;
            }
        }

        public DollarAmount()
        {
            _amt = 0;
        }

        public DollarAmount(double initVal)
        {
            Amt = initVal;
        }

        public override string ToString()
        {
            return _amt.ToString("C"); //formatted as a currency
        }
    }
}
