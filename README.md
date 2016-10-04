# DependentSelection

[![Build status](https://ci.appveyor.com/api/projects/status/xjo4ow9lmybbw6qu?svg=true)](https://ci.appveyor.com/project/emilssonn/dependentselection)
[![Coverage Status](https://coveralls.io/repos/github/emilssonn/DependentSelection/badge.svg)](https://coveralls.io/github/emilssonn/DependentSelection)

## Usage

Create a class that implements the IDependentSelectionFactory interface. Annotate the class with the DependentSelectionFactoryRegistration attribute to register it.

```c#
[DependentSelectionFactoryRegistration]
public class DependentSelectionFactoryDemo : IDependentSelectionFactory
{
  public IEnumerable<ISelectItem> GetSelections(IContentData contentData)
  {
    return new List<ISelectItem>
    {
      new SelectItem
      {
        Text = "1",
        Value = "1"
      }
    };
  }
}
```

The contentData parameter of the GetSelections method is the current content being edited and it is castable to that type.

Annotate a property with the DependentSelectOne(dropdown) or the DependentSelectMany(checkboxes) attribute.

```c#
public class StandardPage : PageData
{
  public virtual string Test1 { get; set; }

  [DependentSelectOne(typeof(DependentSelectionFactoryDemo), DependentOn = new[] { nameof(Test1) })]
  public virtual string Test2 { get; set; }
  
  [DependentSelectMany(typeof(DependentSelectionFactoryDemo), DependentOn = new[] { nameof(Test1) }, ReadOnlyOnEmpty = true)]
  public virtual string Test3 { get; set; }
}
```

When any of the properties in the DependentOn array changes the GetSelections method of the selection factory will be called.
If ReadOnlyOnEmpty is set to true the property will be set to read-only if the list of selections from the GetSelections method is empty.

## Known issues

The attributes does not currently work in local blocks.