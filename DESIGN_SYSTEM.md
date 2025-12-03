# 🎨 Spa Management System - Design System

## Color Palette

```css
Primary:   #454F4A (Dark Teal/Sage Green) - Main brand color, navbar
Secondary: #DCD8CE (Warm Cream/Beige)     - Background, subtle elements  
Accent:    #AA9478 (Warm Tan)             - Active states, highlights
Light:     #F5F3EF (Off-white)            - Page background
Dark:      #2A2F2C (Deep Charcoal)        - Text, dark elements
Success:   #6B9B7E (Soft Green)           - Success messages
Warning:   #D4A574 (Warm Gold)            - Warning messages
Danger:    #C97064 (Muted Red)            - Error messages
Info:      #7A9CAA (Soft Blue)            - Info messages
```

## Layout Structure

### Sidebar (240px width)
- **Background:** var(--spa-primary) #454F4A
- **Top Section:** Brand logo + name
- **Navigation Menu:** Scrollable, active state with accent color
- **Bottom Section:** Logout button

### Main Content Area
- **Top Bar:** White background, page title
- **Content Area:** Light background (#F5F3EF), scrollable

## Component Styles

### Cards
```razor
<div class="card">
    <h3 class="card-title">Card Title</h3>
    <!-- content -->
</div>
```

### Buttons
```razor
<button class="btn btn-primary">Primary Action</button>
<button class="btn btn-secondary">Secondary Action</button>
<button class="btn btn-outline">Outlined Button</button>
```

### Alerts
```razor
<div class="alert alert-success">Success message</div>
<div class="alert alert-danger">Error message</div>
<div class="alert alert-warning">Warning message</div>
<div class="alert alert-info">Info message</div>
```

### Badges
```razor
<span class="badge badge-success">Active</span>
<span class="badge badge-warning">Pending</span>
<span class="badge badge-danger">Cancelled</span>
<span class="badge badge-info">Info</span>
```

### Tables
```razor
<table class="table">
    <thead>
        <tr>
            <th>Column 1</th>
            <th>Column 2</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Data 1</td>
            <td>Data 2</td>
        </tr>
    </tbody>
</table>
```

### Forms
```razor
<div>
    <label class="form-label">Label</label>
    <input class="form-control" type="text" />
</div>
```

## Icons

The system uses Bootstrap Icons. Common icons:

```html
<span class="bi bi-house-door-fill"></span>      <!-- Home -->
<span class="bi bi-people-fill"></span>          <!-- Customers -->
<span class="bi bi-calendar-fill"></span>        <!-- Appointments -->
<span class="bi bi-scissors"></span>             <!-- Services -->
<span class="bi bi-box-seam"></span>             <!-- Products -->
<span class="bi bi-cart-fill"></span>            <!-- Sales -->
<span class="bi bi-graph-up"></span>             <!-- Reports -->
<span class="bi bi-gear-fill"></span>            <!-- Settings -->
<span class="bi bi-database"></span>             <!-- Database -->
<span class="bi bi-check-circle-fill"></span>    <!-- Success -->
<span class="bi bi-x-circle-fill"></span>        <!-- Error -->
<span class="bi bi-info-circle-fill"></span>     <!-- Info -->
```

## Typography

- **Headings:** Segoe UI, bold, color: var(--spa-primary)
- **Body Text:** Segoe UI, regular, color: var(--spa-text-dark)
- **Subtle Text:** color: var(--spa-text-light)

## Spacing

```css
.mt-1, .mb-1  → 0.5rem
.mt-2, .mb-2  → 1rem
.mt-3, .mb-3  → 1.5rem
.p-1          → 0.5rem
.p-2          → 1rem
.p-3          → 1.5rem
```

## CSS Variables

Use these in your components:

```css
var(--spa-primary)      /* #454F4A */
var(--spa-secondary)    /* #DCD8CE */
var(--spa-accent)       /* #AA9478 */
var(--spa-light)        /* #F5F3EF */
var(--spa-dark)         /* #2A2F2C */
var(--spa-text-dark)    /* #333333 */
var(--spa-text-light)   /* #666666 */
var(--spa-success)      /* #6B9B7E */
var(--spa-warning)      /* #D4A574 */
var(--spa-danger)       /* #C97064 */
var(--spa-info)         /* #7A9CAA */
```

## Example Page Layout

```razor
@page "/example"

<PageTitle>Example Page</PageTitle>

<div class="card">
    <h3 class="card-title">Page Title</h3>
    
    <div class="alert alert-info">
        <span class="bi bi-info-circle-fill"></span>
        <div>Important information here</div>
    </div>
    
    <button class="btn btn-primary">
        <span class="bi bi-plus-circle"></span> Add New
    </button>
</div>

<div class="card">
    <h4 class="card-title">Data Table</h4>
    
    <table class="table">
        <thead>
            <tr>
                <th>Name</th>
                <th>Status</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>John Doe</td>
                <td><span class="badge badge-success">Active</span></td>
                <td>
                    <button class="btn btn-outline">Edit</button>
                </td>
            </tr>
        </tbody>
    </table>
</div>
```

## Grid Layouts

For responsive card grids:

```html
<div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 1.5rem;">
    <div class="card">Card 1</div>
    <div class="card">Card 2</div>
    <div class="card">Card 3</div>
</div>
```

## Navigation Menu Structure

Current menu items (can be expanded):
- 🏠 Home (/)
- 🗄️ DB Test (/db-test)
- ➕ Counter (/counter)
- ☁️ Weather (/weather)

### Adding New Menu Items

Edit `Components/Layout/NavMenu.razor`:

```razor
<div class="nav-item">
    <NavLink class="nav-link" href="/your-page">
        <span class="bi bi-your-icon"></span> Page Name
    </NavLink>
</div>
```

## Best Practices

1. **Always use cards** for content sections
2. **Use consistent spacing** between elements
3. **Include icons** with labels for better UX
4. **Use semantic colors** (success for positive, danger for negative)
5. **Keep the color palette** throughout the application
6. **Use tables** for data display
7. **Use badges** for status indicators
8. **Use alerts** for important messages

## Responsive Design

The layout automatically adapts:
- **Desktop:** Full sidebar visible
- **Mobile:** Sidebar becomes a drawer (ready for mobile menu toggle)

---

**Your spa management system now has a cohesive, professional design! 🌿✨**
