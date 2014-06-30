class Dashing.ServiceStatus extends Dashing.Widget
  @accessor 'background-class'
  @accessor 'background-icon-class'

  onData: (data) ->
    if data.is_okay 
        @set 'background-class', 'okay'
        @set 'background-icon-class', 'icon-check-circle-o icon-background'
    else
        @set 'background-class', 'error'
        @set 'background-icon-class', 'icon-ban-circle icon-background'