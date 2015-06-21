# -*- mode: ruby -*-
# vi: set ft=ruby :

require 'tempfile'

all_versions = ["9.0", "9.1", "9.2", "9.3", "9.4"]
default = "latest"
vms = {
  default => [all_versions.last],
  "all"   => all_versions
}

vms.each do |name, versions|
  minion_config = Tempfile.new("minion")
  minion_config.write("file_client: local\n")
  minion_config.write("grains:\n")
  minion_config.write("  pgsql_versions:\n")
  versions.each do |version|
    minion_config.write("    - #{version}\n")
  end
  minion_config.close
  vms[name] = { "versions" => versions, "minion_config" => minion_config }
end

Vagrant.configure(2) do |config|
  vms.each do |name, data|
    config.vm.define name, primary: name == default, autostart: name == default do |node|
      node.vm.box = "ubuntu/trusty64"

      data["versions"].each do |version|
        port = 5400 + version.delete('.').to_i
        node.vm.network "forwarded_port", guest: port, host: port
      end

      node.vm.synced_folder ".", "/vagrant", disabled: true

      node.vm.synced_folder "salt", "/srv/salt"

      node.vm.provision :salt do |salt|
        salt.minion_config = data["minion_config"].path
        salt.run_highstate = true
      end
    end
  end
end
